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
using System.Text;
using System.Text.RegularExpressions;

namespace net.r_eg.vsSBE.SBEScripts
{
    public static class RPattern
    {
        /// <summary>
        /// Captures content from double quotes
        /// </summary>
        public static string DoubleQuotesContent
        {
            get {
                return quotesContent('"');
            }
        }

        /// <summary>
        /// Captures content from single quotes
        /// </summary>
        public static string SingleQuotesContent
        {
            get {
                return quotesContent('\'');
            }
        }

        /// <summary>
        /// Captures content from Square Brackets
        /// [ ... ]
        /// </summary>
        public static string SquareBracketsContent
        {
            get { return bracketsContent('[', ']'); }
        }

        /// <summary>
        /// Captures content from Parentheses (Round Brackets)
        /// ( ... )
        /// </summary>
        public static string RoundBracketsContent
        {
            get { return bracketsContent('(', ')'); }
        }

        /// <summary>
        /// Captures content from Curly Brackets
        /// { ... }
        /// </summary>
        public static string CurlyBracketsContent
        {
            get { return bracketsContent('{', '}'); }
        }

        /// <summary>
        /// Captures content for present symbol of quotes
        /// Escaping is a "\" for used symbol
        /// e.g.: \', \"
        /// </summary>
        /// <param name="symbol">' or "</param>
        private static string quotesContent(char symbol)
        {
            return String.Format(@"
                                  \s*(?<!\\){0}
                                  (
                                     (?:
                                        [^{0}\\]
                                      |
                                        \\{0}?
                                     )*
                                  )
                                  {0}\s*", symbol);
        }


        /// <summary>
        /// Captures content for present symbol of brackets
        /// 
        /// Note: A balancing group definition deletes the definition of a previously defined group, 
        ///       therefore allowed some intersection with name of the balancing group.. don't worry., be happy
        /// </summary>
        /// <param name="open">left symbol of bracket: [, {, ( etc.</param>
        /// <param name="close">right symbol of bracket: ], }, ) etc.</param>
        private static string bracketsContent(char open, char close)
        {
            return String.Format(@"\{0}
                                   (
                                     (?>
                                       [^\{0}\{1}]
                                       |
                                       \{0}(?<R>)
                                       |
                                       \{1}(?<-R>)
                                     )*
                                     (?(R)(?!))
                                   )
                                   \{1}", open, close);
        }
    }
}
