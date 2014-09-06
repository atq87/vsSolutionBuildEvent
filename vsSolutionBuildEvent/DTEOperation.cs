﻿/* 
 * Boost Software License - Version 1.0 - August 17th, 2003
 * 
 * Copyright (c) 2013-2014 Developed by reg <entry.reg@gmail.com>
 * 
 * Permission is hereby granted, free of charge, to any person or organization
 * obtaining a copy of the software and accompanying documentation covered by
 * this license (the "Software") to use, reproduce, display, distribute,
 * execute, and transmit the Software, and to prepare derivative works of the
 * Software, and to permit third-parties to whom the Software is furnished to
 * do so, all subject to the following:
 * 
 * The copyright notices in the Software and this entire statement, including
 * the above license grant, this restriction and the following disclaimer,
 * must be included in all copies of the Software, in whole or in part, and
 * all derivative works of the Software, unless such copies or derivative
 * works are solely in the form of machine-executable object code generated by
 * a source language processor.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT
 * SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE
 * FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE. 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EnvDTE;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE
{
    public class DTEOperation
    {
        /// <summary>
        /// Aggregation of prepared data for DTE
        /// </summary>
        public struct TPrepared
        {
            /// <summary>
            /// The name of the command to invoke.
            /// </summary>
            public string name;
            /// <summary>
            /// A string containing the arguments for DTE-command.
            /// see format with _DTE.ExecuteCommand
            /// </summary>
            public string args;

            public TPrepared(string name, string args)
            {
                this.name = name;
                this.args = args;
            }
        }

        /// <summary>
        /// Support recursive DTE-commands with level protection
        /// e.g.:
        ///   exec - "Debug.Start"
        ///   exec - "Debug.Start"
        ///   exec - "File.Print"
        /// </summary>
        protected struct TQueue
        {
            public uint level;
            public Queue<TPrepared> cmd;
        }
        protected TQueue queue;

        /// <summary>
        /// DTE context
        /// </summary>
        protected DTE dte;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _eLock = new Object();

        public virtual TPrepared parse(string line)
        {
            Match m = Regex.Match(line.Trim(), @"^
                                                   ([A-z_0-9.]+)    #1 - Command
                                                   (?:
                                                       \s*
                                                       \(
                                                          ([^)]+)   #2 - Arguments (optional)
                                                       \)           ## http://msdn.microsoft.com/en-us/library/envdte._dte.executecommand.aspx
                                                       \s*
                                                   )?
                                                 $", RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                Log.nlog.Debug("Operation '{0}' is not correct", line);
                throw new IncorrectSyntaxException(String.Format("prepare failed - '{0}'", line));
            }
            return new TPrepared(m.Groups[1].Value, m.Groups[2].Success ? m.Groups[2].Value.Trim() : String.Empty);
        }

        public Queue<TPrepared> parse(string[] raw)
        {
            Queue<TPrepared> pRaw = new Queue<TPrepared>();

            foreach(string rawLine in raw) {
                pRaw.Enqueue(parse(rawLine));
            }
            return pRaw;
        }

        public void exec(TPrepared command)
        {
            exec(command.name, command.args);
        }

        public void exec(string[] commands, bool abortOnFirstError)
        {
            exec(parse(format(ref commands)), abortOnFirstError);
        }

        public void exec(Queue<TPrepared> commands, bool abortOnFirstError)
        {
            if(queue.level < 1 && commands.Count < 1) {
                return;
            }

            lock(_eLock)
            {
                if(queue.level == 0) {
                    queue.cmd = commands;
                }

                ++queue.level;

                TPrepared current       = queue.cmd.Dequeue();
                string progressCaption  = String.Format("({0}/{1})", queue.level, queue.level + queue.cmd.Count);
                Exception terminated    = null;
                try {
                    // also error if command not available at current time
                    // * +causes recursion with Debug.Start, Debug.StartWithoutDebugging, etc.,
                    Log.nlog.Info("DTE exec {0}: '{1}'", progressCaption, current.name);
                    exec(current.name, current.args);
                }
                catch(Exception ex) {
                    Log.nlog.Debug("DTE fail {0}: {1} :: '{2}'", progressCaption, ex.Message, current.name);
                    terminated = ex;
                }

                if(queue.cmd.Count > 0)
                {
                    // remaining commands
                    if(terminated != null && abortOnFirstError) {
                        Log.nlog.Info("DTE exec {0}: Aborted", progressCaption);
                    }
                    else {
                        Log.nlog.Debug("DTE {0}: step into", progressCaption);
                        exec((Queue<TPrepared>)null, abortOnFirstError);
                    }
                }

                --queue.level;

                if(queue.level < 1 && terminated != null) {
                    throw new ComponentException(terminated.Message, terminated);
                }
            }
        }

        public virtual void exec(string name, string args)
        {
            dte.ExecuteCommand(name, args);
        }

        public DTEOperation(DTE dte)
        {
            this.dte    = dte;
            queue       = new TQueue();
        }

        protected virtual string[] format(ref string[] data)
        {
            return data.Where(s => s.Trim().Length > 0).ToArray();
        }
    }
}
