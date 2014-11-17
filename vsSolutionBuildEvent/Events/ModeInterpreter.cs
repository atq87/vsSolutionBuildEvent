﻿/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Processing with streaming tools
    /// </summary>
    public class ModeInterpreter: IMode, IModeInterpreter
    {
        /// <summary>
        /// Type of implementation
        /// </summary>
        public ModeType Type
        {
            get { return ModeType.Interpreter; }
        }

        /// <summary>
        /// Command for handling
        /// </summary>
        public string Command
        {
            get { return command; }
            set { command = value; }
        }
        private string command = string.Empty;

        /// <summary>
        /// Stream handler
        /// </summary>
        public string Handler
        {
            get { return handler; }
            set { handler = value; }
        }
        private string handler = string.Empty;

        /// <summary>
        /// Treat newline as
        /// </summary>
        public string Newline
        {
            get { return newline; }
            set { newline = value; }
        }
        private string newline = string.Empty;

        /// <summary>
        /// Symbol/s for wrapping of commands
        /// </summary>
        public string Wrapper
        {
            get { return wrapper; }
            set { wrapper = value; }
        }
        private string wrapper = string.Empty;
    }
}