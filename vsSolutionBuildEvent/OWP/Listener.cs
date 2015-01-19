﻿/*
 * Copyright (c) 2013  Denis Kuzmin (reg) <entry.reg@gmail.com>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using System.Threading;

namespace net.r_eg.vsSBE.OWP
{
    /// <summary>
    /// Working with the OutputWindowsPane
    /// Must receive and send different data for own subscribers
    /// </summary>
    internal class Listener: SynchSubscribers<IListenerOWPL>
    {
        /// <summary>
        /// Keep events for any pane
        /// </summary>
        protected OutputWindowEvents evtOWP;

        //TODO: fix me. Prevent Duplicate Data / bug with OutputWindowPane
        protected SynchronizedCollection<string> dataList = new SynchronizedCollection<string>();
        protected System.Threading.Thread tUpdated;

        /// <summary>
        /// Used item by name
        /// </summary>
        protected string item;

        /// <summary>
        /// previous count of lines for EditPoint::GetLines
        /// </summary>
        private int _prevCountLines = 1;

        /// <summary>
        /// obj synch.
        /// </summary>
        private Object _eLock = new Object();

        public void attachEvents()
        {
            if(evtOWP == null) {
                Log.nlog.Warn("OWP: Disabled for current Environment.");
                return;
            }

            lock(_eLock) {
                detachEvents();
                evtOWP.PaneUpdated      += new _dispOutputWindowEvents_PaneUpdatedEventHandler(evtPaneUpdated);
                evtOWP.PaneAdded        += new _dispOutputWindowEvents_PaneAddedEventHandler(evtPaneAdded);
                evtOWP.PaneClearing     += new _dispOutputWindowEvents_PaneClearingEventHandler(evtPaneClearing);
            }
        }

        public void detachEvents()
        {
            if(evtOWP == null) {
                return;
            }
            lock(_eLock) {
                evtOWP.PaneUpdated      -= new _dispOutputWindowEvents_PaneUpdatedEventHandler(evtPaneUpdated);
                evtOWP.PaneAdded        -= new _dispOutputWindowEvents_PaneAddedEventHandler(evtPaneAdded);
                evtOWP.PaneClearing     -= new _dispOutputWindowEvents_PaneClearingEventHandler(evtPaneClearing);
            }
        }

        public Listener(IEnvironment env, string item)
        {
            this.item = item;

            if(env.Events != null) {
                evtOWP = env.Events.get_OutputWindowEvents(item);
            }
        }

        /// <summary>
        /// all collection must receive raw-data
        /// TODO: fix me. Prevent Duplicate Data / bug with OutputWindowPane
        /// </summary>
        protected virtual void notifyRaw()
        {
            if(dataList.Count < 1) {
                return;
            }

            lock(_eLock)
            {
                string envelope = "";
                while(dataList.Count > 0) {
                    envelope += dataList[0];
                    dataList.RemoveAt(0);
                }

                updateComponent(envelope);
                foreach(IListenerOWPL l in subscribers) {
                    l.raw(envelope);
                }
            }

            if(dataList.Count > 0) {
                notifyRaw();
            }
        }

        protected virtual void evtPaneUpdated(OutputWindowPane pane)
        {
            TextDocument textD   = pane.TextDocument;
            int countLines       = textD.EndPoint.Line;

            if(countLines <= 1 || countLines - _prevCountLines < 1) {
                return;
            }

            EditPoint point = textD.StartPoint.CreateEditPoint();

            // text between Start (inclusive) and ExclusiveEnd (exclusive)
            dataList.Add(point.GetLines(_prevCountLines, countLines)); // e.g. first line: 1, 2
            _prevCountLines = countLines;

            //TODO: fix me. Prevent Duplicate Data / bug with OutputWindowPane
            if(tUpdated == null || tUpdated.ThreadState == ThreadState.Unstarted || tUpdated.ThreadState == ThreadState.Stopped)
            {
                tUpdated = new System.Threading.Thread(() => { notifyRaw(); });
                try {
                    tUpdated.Start();
                }
                catch(Exception e) {
                    Log.nlog.Warn("notifyRaw() {0}", e.Message);
                }
            }
        }

        protected virtual void evtPaneAdded(OutputWindowPane pane)
        {
            _prevCountLines = 1;
            dataList.Clear();
        }

        protected virtual void evtPaneClearing(OutputWindowPane pane)
        {
            _prevCountLines = 1;
            dataList.Clear();
        }

        protected void updateComponent(string data)
        {
            switch(item) {
                case "Build": {
                    Items._.Build.updateRaw(data);
                    return;
                }
            }
        }
    }
}
