﻿/*
*   This file is part of NSMB Editor 5.
*
*   NSMB Editor 5 is free software: you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*   (at your option) any later version.
*
*   NSMB Editor 5 is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with NSMB Editor 5.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace NSMBe4
{
    public partial class PathEditor : UserControl
    {
        private LevelEditorControl EdControl;
        private bool DataUpdateFlag = false;
        private bool isProgress;
        List<LevelItem> SelectedObjects;
        List<NSMBPath> lst;

        public PathEditor(LevelEditorControl EdControl, List<NSMBPath> l)
        {
            InitializeComponent();
            this.lst = l;
            isProgress = l == EdControl.Level.ProgressPaths;
            LanguageManager.ApplyToContainer(this, "PathEditor");
            this.EdControl = EdControl;
            UpdateList();
        }

        public void SelectObjects(List<LevelItem> objs)
        {
            SelectedObjects = objs;
            UpdateInfo();
        }

        public void UpdateList()
        {
            DataUpdateFlag = true;
            pathsList.Items.Clear();
            pathsList.Items.AddRange(lst.ToArray());
            DataUpdateFlag = false;
        }

        public void UpdateInfo()
        {
            UpdateList();

            if (SelectedObjects == null || SelectedObjects.Count == 0)
            {
                panel2.Visible = false;
                deletePath.Enabled = false;
                return;
            }
            NSMBPathPoint pp = SelectedObjects[0] as NSMBPathPoint;
            panel2.Visible = true;
            deletePath.Enabled = true;
            DataUpdateFlag = true;
            List<NSMBPath> paths = new List<NSMBPath>();
            foreach (LevelItem obj in SelectedObjects) {
                NSMBPath p = (obj as NSMBPathPoint).parent;
                if (lst.Contains(p) && !paths.Contains(p))
                {
                    pathsList.SelectedIndices.Add(lst.IndexOf(p));
                    paths.Add(p);
                }
            }

            pathID.Value = pp.parent.id;
            unk1.Value = pp.Unknown1;
            unk2.Value = pp.Unknown2;
            unk3.Value = pp.Unknown3;
            unk4.Value = pp.Unknown4;
            unk5.Value = pp.Unknown5;
            unk6.Value = pp.Unknown6;
            DataUpdateFlag = false;
        }

        private void pathID_ValueChanged(object sender, EventArgs e)
        {
            if (DataUpdateFlag) return;
            EdControl.UndoManager.Do(new ChangePathIDAction(SelectedObjects, (int)pathID.Value));
        }

        private void unk1_ValueChanged(object sender, EventArgs e)
        {
            if (DataUpdateFlag) return;
            EdControl.UndoManager.Do(new ChangePathNodeDataAction(SelectedObjects, 0, (int)unk1.Value));
        }

        private void unk2_ValueChanged(object sender, EventArgs e)
        {
            if (DataUpdateFlag) return;
            EdControl.UndoManager.Do(new ChangePathNodeDataAction(SelectedObjects, 1, (int)unk2.Value));
        }

        private void unk3_ValueChanged(object sender, EventArgs e)
        {
            if (DataUpdateFlag) return;
            EdControl.UndoManager.Do(new ChangePathNodeDataAction(SelectedObjects, 2, (int)unk3.Value));
        }

        private void unk4_ValueChanged(object sender, EventArgs e)
        {
            if (DataUpdateFlag) return;
            EdControl.UndoManager.Do(new ChangePathNodeDataAction(SelectedObjects, 3, (int)unk4.Value));
        }

        private void unk5_ValueChanged(object sender, EventArgs e)
        {
            if (DataUpdateFlag) return;
            EdControl.UndoManager.Do(new ChangePathNodeDataAction(SelectedObjects, 4, (int)unk5.Value));
        }

        private void unk6_ValueChanged(object sender, EventArgs e)
        {
            if (DataUpdateFlag) return;
            EdControl.UndoManager.Do(new ChangePathNodeDataAction(SelectedObjects, 5, (int)unk6.Value));
        }

        private void pathsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DataUpdateFlag) return;
            DataUpdateFlag = true;
            List<LevelItem> pts = new List<LevelItem>();
            for (int l = 0; l < pathsList.SelectedIndices.Count; l++)
                foreach (NSMBPathPoint pp in lst[pathsList.SelectedIndices[l]].points)
                    pts.Add(pp);
            if (pts.Count == 0)
                EdControl.SelectObject(null);
            else {
                EdControl.SelectObject(pts);
                EdControl.ScrollToObjects(pts);
            }
            DataUpdateFlag = false;
            EdControl.repaint();
        }

        private void addPath_Click(object sender, EventArgs e)
        {
            Rectangle va = EdControl.ViewableBlocks;
            NSMBPath np = new NSMBPath();
            if (isProgress)
                np.id = EdControl.Level.getFreePathNumber(lst, 1);
            else
                np.id = EdControl.Level.getFreePathNumber(lst, 0);
            np.isProgressPath = isProgress;

            NSMBPathPoint npp = new NSMBPathPoint(np);
            npp.X = (va.X + va.Width / 2) * 16;
            npp.Y = (va.Y + va.Height / 2) * 16;
            EdControl.UndoManager.Do(new AddLvlItemAction(UndoManager.ObjToList(npp)));
            EdControl.mode.SelectObject(npp);
        }

        private void deletePath_Click(object sender, EventArgs e)
        {
            List<LevelItem> points = new List<LevelItem>();
            foreach (LevelItem obj in SelectedObjects)
                if (obj is NSMBPathPoint)
                    points.Add(obj);
            foreach (LevelItem obj in points)
                SelectedObjects.Remove(obj);
            EdControl.UndoManager.Do(new RemoveLvlItemAction(points));
        }
    }
}
