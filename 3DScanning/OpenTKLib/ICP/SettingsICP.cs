using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Windows.Media;
using System.Diagnostics;
using OpenTK;
using OpenTKLib;


namespace ICPLib
{
    public class SettingsICP
    {
        public ICP_VersionUsed ICPVersion = ICP_VersionUsed.Scaling_Umeyama;
        public KDTreeMode KDTreeMode;
        public double ConvergenceThreshold = 1e-4;

        public int MaxNumberSolutions = 10;
        public int NumberOfStartTrialPoints = 100000;

        public bool SimulatedAnnealing = false;
        public bool Normal_RemovePoints = false;
        public bool Normal_SortPoints = false;

        public bool FixedTestPoints = false;
        public int MaximumNumberOfIterations = 100;
        public bool ResetVertexToOrigin = true;
        public bool DistanceOptimization = false;
        
        public bool PerformInitial2DICP = false;
        public bool ShuffleEffect = true;
        public double MaximumMeanDistance;
        public double ThresholdOutlier = 10;
        
       
    }
}
