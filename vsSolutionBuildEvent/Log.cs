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
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.Globalization;
using NLog;

namespace net.r_eg.vsSBE
{
    public class Log
    {
        public const string OWP_ITEM_NAME = "Solution Build-Events";

        /// <summary>
        /// external logic
        /// </summary>
        public static readonly Logger nlog = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// to display text output, represented by the OutputWindowPane
        /// </summary>
        protected static OutputWindowPane pane = null;

        /// <summary>
        /// NLog :: static "MethodCall"
        /// use with nlog
        /// https://github.com/nlog/nlog/wiki/MethodCall-target
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="stamp"></param>
        public static void _print(string level, string message, string stamp)
        {
            string format = String.Format(
                                "{0} [{1}]: {2}{3}",
                                (new DateTime(long.Parse(stamp))).ToString(CultureInfo.CurrentCulture.DateTimeFormat),
                                level,
                                message,
                                Environment.NewLine);
            print(format);
        }

        public static void print(string message)
        {
            if(pane == null) {
                Console.WriteLine(message);
                return;
            }
            pane.OutputString(message);
        }

        /// <summary>
        /// if you want to use OWP
        /// </summary>
        /// <param name="dte"></param>
        public static void init(DTE2 dte)
        {
            try {
                pane = dte.ToolWindows.OutputWindow.OutputWindowPanes.Item(OWP_ITEM_NAME);
            }
            catch(ArgumentException) {
                pane = dte.ToolWindows.OutputWindow.OutputWindowPanes.Add(OWP_ITEM_NAME);
            }
            catch(Exception e) {
                throw new Exception("Log :: inner exception", e);
            }
        }

        public static void show()
        {
            pane.Activate();
        }

        protected Log() { }
    }
}
