using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics; // Stopwatch

namespace CfopTrainer {
    enum Side {
        Left
        , Front
        , Right
        , Back
        , Up
        , Down
    }

    class Practicer {
        protected Random random;
        protected Cube cube;
        protected Stopwatch watch;

        protected Practicer(Cube cube) {
            this.cube = cube; // several practicers, and the only one cube
            random = new Random();
        }//fn
    }//cl
}//ns
