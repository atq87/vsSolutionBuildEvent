﻿/* 
 * Boost Software License - Version 1.0 - August 17th, 2003
 * 
 * Copyright (c) 2013-2014 Developed by reg [Denis Kuzmin] <entry.reg@gmail.com>
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace net.r_eg.vsSBE.SBEScripts
{
    public class StringHandler
    {
        /// <summary>
        /// Storage of protected strings
        /// Contains the all found strings from data
        /// </summary>
        protected ConcurrentDictionary<uint, string> strings = new ConcurrentDictionary<uint, string>();

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// Protecting the all strings for data
        /// For work with depth, we need protect data from intersections with data from strings
        /// </summary>
        /// <param name="data"></param>
        /// <returns>safety data</returns>
        public string protect(string data)
        {
            lock(_lock)
            {
                uint ident = 0;
                strings.Clear();

                return Regex.Replace(data, String.Format(@"({0}|{1})",                     // #1 - mixed
                                                            RPattern.DoubleQuotesContent,  // #2 -  ".." 
                                                            RPattern.SingleQuotesContent), // #3 -  '..'
                delegate(Match m)
                {
                    strings[ident] = m.Groups[1].Value;
                    Log.nlog.Trace("StringHandler: protect string '{0}' :: '{1}'", strings[ident], ident);

                    // no conflict, because all variants with '!' as argument is not possible without quotes.
                    return String.Format("!s{0}!", ident++);
                },
                RegexOptions.IgnorePatternWhitespace);
            }
        }

        /// <summary>
        /// Restoring protected strings after handling protectStrings()
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string recovery(string data)
        {
            Debug.Assert(strings != null);

            lock(_lock)
            {
                return Regex.Replace(data, @"!s(\d+)!", delegate(Match m)
                {
                    string removed;
                    uint index = uint.Parse(m.Groups[1].Value);
                    strings.TryRemove(index, out removed); // deallocate protected string

                    Log.nlog.Trace("StringHandler: recovery string '{0}' :: '{1}'", removed, index);
                    return removed;
                });
            }
        }

        /// <summary>
        /// Normalization data of strings with escaped double quotes etc.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string normalize(string data)
        {
            return data.Replace("\\\"", "\"");
        }
    }
}
