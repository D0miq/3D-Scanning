using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenTKLib
{
  public static class Parser
    {

      public static Matrix3d Substitute(string baseMatrix, string stringToSubtitute, double substitutionValue)
      {
          string working = baseMatrix.Replace(stringToSubtitute, substitutionValue.ToString("R"));
          Matrix3d mat = new Matrix3d();
          return mat.Parse(working);

      }
    }
}
