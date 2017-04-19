using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TwoSidePLL { 
    public partial class MainWindow : Window {
        Cube theCube;
        Practicer practicer;

        public MainWindow() {
            try
            {
                InitializeComponent();

                initializeFrontView();
                initializeBackView();
                initializeScrambles();
                initializeViews();
       
                theCube = new Cube();
                practicer = new Practicer(this, theCube);
                updateGui();
            }
            catch (System.NullReferenceException ex)
            {
                MessageBox.Show(ex.Message);
            }
             
        }
        private void click_startPractice(object sender, RoutedEventArgs e) {
            int practiceLength = Convert.ToInt32((comPracticeSize.SelectedItem as ComboBoxItem).Content);
            practicer.newPractice(practiceLength);
            if (practicer.nextScramble())  {// generate next scramble
                updateGui();                // show it
                practicer.Running = true;
                progressBar1.Minimum = 1;
                progressBar1.Maximum = practicer.tasks.Count;
                progressBar1.Value = 1;
            }
        }
        private void keyDown(object sender, KeyEventArgs e) {
            if (practicer.Running) {
                Title = e.Key.ToString();
                char ch = char.ToLower(Title[0]);

                if (practicer.verify(ch)) { // verify 
                    pluser.Content += " +";// set result
                } else {
                    minuser.Content += " " + practicer.Correct.ToString();
                    
                    // TODO: add to penalties AS;DLFKASD;LFKASD';LFKASLDKJFA;SLDKJFA;SLDKJFA;SLDKFJA;LSKJ423O1897491287491
                    //practicer.penalties.Enqueue(practicer.Correct.ToUpper()[0].ToString());
                    //progressBar1.Maximum++;
                }//ei

                if(practicer.nextScramble()) {// go next guess
                    updateGui();               // show it
                    
                } else {
                    practicer.Running = false;
                }//ei
            }//if
        }
        private void click_group_Correct(object sender, RoutedEventArgs e)
        {
            cbH.IsChecked = cbCorrect.IsChecked;
            cbUa.IsChecked = cbCorrect.IsChecked;
            cbUb.IsChecked = cbCorrect.IsChecked;
            cbZ.IsChecked = cbCorrect.IsChecked;
        }
        private void click_group_Diagonal(object sender, RoutedEventArgs e)
        {
            cbV.IsChecked = cbDiagonal.IsChecked;
            cbY.IsChecked = cbDiagonal.IsChecked;
            cbNa.IsChecked = cbDiagonal.IsChecked;
            cbNb.IsChecked = cbDiagonal.IsChecked;
            cbE.IsChecked = cbDiagonal.IsChecked;
        }
        private void click_group_Anjacent(object sender, RoutedEventArgs e)
        {
            cbGa.IsChecked = cbAdjacent.IsChecked;
            cbGb.IsChecked = cbAdjacent.IsChecked;
            cbGc.IsChecked = cbAdjacent.IsChecked;
            cbGd.IsChecked = cbAdjacent.IsChecked;
            cbAa.IsChecked = cbAdjacent.IsChecked;
            cbAb.IsChecked = cbAdjacent.IsChecked;
            cbT.IsChecked  = cbAdjacent.IsChecked;
            cbJa.IsChecked = cbAdjacent.IsChecked;
            cbJb.IsChecked = cbAdjacent.IsChecked;
            cbRa.IsChecked = cbAdjacent.IsChecked;
            cbRb.IsChecked = cbAdjacent.IsChecked;
            cbF.IsChecked  = cbAdjacent.IsChecked;
        }
        private void click_group_View(object sender, RoutedEventArgs e)
        {
            cbLeft.IsChecked    = cbView.IsChecked;
            cbRight.IsChecked   = cbView.IsChecked;
            cbBack.IsChecked    = cbView.IsChecked;
            cbFace.IsChecked    = cbView.IsChecked;
           //cbTurnF2L.IsChecked = cbView.IsChecked;
        }
        private void click_group_Topcolor(object sender, RoutedEventArgs e)
        {
            cbYellow.IsChecked = cbTopcolors.IsChecked;
            cbWhite.IsChecked  = cbTopcolors.IsChecked;
            cbRed.IsChecked    = cbTopcolors.IsChecked;
            cbGreen.IsChecked  = cbTopcolors.IsChecked;
            cbOrange.IsChecked = cbTopcolors.IsChecked;
            cbBlue.IsChecked   = cbTopcolors.IsChecked;
        }
        private void click_execute(object sender, RoutedEventArgs e)
        {
            Title = theCube.execute(tbMovesTodo.Text);
            updateGui();
        }
        private void click_reset(object sender, RoutedEventArgs e)
        {
            theCube.resetStickers();
            updateGui();
        }
    }//cl

    partial class Practicer {
        Cube cube;
        CubeManip cubeManipulator;
        internal Queue<string> tasks     = new Queue<string>();
        internal Queue<string> penalties = new Queue<string>();

        public Practicer(MainWindow form, Cube cube) {
            this.form = form;
            this.cube = cube;
            cubeManipulator = new CubeManip();
        }
        public void newPractice(int tasksToGenerate)
        { // generate new set of tasks
            Random random = new Random();
            List<CheckBox> selections = form.getSelectedScrambles();
            for (/**/; tasksToGenerate > 0; tasksToGenerate--) {
                int randomIndex = random.Next(0, selections.Count);
                string text = selections[randomIndex].Content.ToString();
                tasks.Enqueue(text);
            }//for
        }
        public bool nextScramble() {
            var random = new Random();
            var lists = new List<Queue<string>>();
            if(tasks.Count > 0) {     lists.Add(tasks);     }
            if(penalties.Count > 0) { lists.Add(penalties); }
            if (lists.Count > 0) {
                int index = random.Next(0, lists.Count);

                string newTaskTodo = lists[index].Dequeue();
                MessageBox.Show(newTaskTodo);
                cube.resetStickers(); // TODO: reset to random color in dependence of the checkboxes
                Correct = cubeManipulator.scrambleTheCube(cube, newTaskTodo); // generate the scramble;

                //AUF/AUF part
                var selectedViews = form.getSelectedViews();
                int aufIndx = random.Next(0, selectedViews.Count);
                cube.execute(selectedViews[aufIndx]); // AUF
                
                 // F2l-AUF is fixed 4 rotations max
                string auf2Task = "U ";
                for (aufIndx = random.Next(0, 4); aufIndx > 0; aufIndx--) {
                    auf2Task += "U ";
                }
                cube.execute(auf2Task); // F2L-Auf
                return true; // got a task
            }//if
            return false; // got no more tasks
        }
        public bool verify(char ch) {
            return ch.ToString().ToUpper()[0] == Correct[0];
        }

    }//cl
    
    public partial class MainWindow : Window
    {
        Dictionary<Side, List<Polygon>> uiData;
        Dictionary<Side, List<Rectangle>> uiBackView;
        List<CheckBox> scrambles = new List<CheckBox>(); // i think it should be a part of practicer
        Dictionary<CheckBox, string/*AUF*/> views = new Dictionary<CheckBox, string>();     // AUF for t
        List<CheckBox> topColors = new List<CheckBox>(); //

        private void initializeFrontView()
        {
            uiData = new Dictionary<Side, List<Polygon>>();
            uiData.Add(Side.Front, new List<Polygon>());
            uiData.Add(Side.Right, new List<Polygon>());
            uiData.Add(Side.Up, new List<Polygon>());
            uiData[Side.Front].Add(face1);
            uiData[Side.Front].Add(face2);
            uiData[Side.Front].Add(face3);
            uiData[Side.Front].Add(face4);
            uiData[Side.Front].Add(face5);
            uiData[Side.Front].Add(face6);
            uiData[Side.Front].Add(face7);
            uiData[Side.Front].Add(face8);
            uiData[Side.Front].Add(face9);
            uiData[Side.Right].Add(side1);
            uiData[Side.Right].Add(side2);
            uiData[Side.Right].Add(side3);
            uiData[Side.Right].Add(side4);
            uiData[Side.Right].Add(side5);
            uiData[Side.Right].Add(side6);
            uiData[Side.Right].Add(side7);
            uiData[Side.Right].Add(side8);
            uiData[Side.Right].Add(side9);
            uiData[Side.Up].Add(ceil1);
            uiData[Side.Up].Add(ceil2);
            uiData[Side.Up].Add(ceil3);
            uiData[Side.Up].Add(ceil4);
            uiData[Side.Up].Add(ceil5);
            uiData[Side.Up].Add(ceil6);
            uiData[Side.Up].Add(ceil7);
            uiData[Side.Up].Add(ceil8);
            uiData[Side.Up].Add(ceil9);
        }
        private void initializeBackView()
        {
            uiBackView = new Dictionary<Side, List<Rectangle>>();
            uiBackView.Add(Side.Left, new List<Rectangle>());
            uiBackView.Add(Side.Back, new List<Rectangle>());
            uiBackView.Add(Side.Down, new List<Rectangle>());
            uiBackView[Side.Left].Add(left0);
            uiBackView[Side.Left].Add(left1);
            uiBackView[Side.Left].Add(left2);
            uiBackView[Side.Left].Add(left3);
            uiBackView[Side.Left].Add(left4);
            uiBackView[Side.Left].Add(left5);
            uiBackView[Side.Left].Add(left6);
            uiBackView[Side.Left].Add(left7);
            uiBackView[Side.Left].Add(left8);
            uiBackView[Side.Back].Add(back0);
            uiBackView[Side.Back].Add(back1);
            uiBackView[Side.Back].Add(back2);
            uiBackView[Side.Back].Add(back3);
            uiBackView[Side.Back].Add(back4);
            uiBackView[Side.Back].Add(back5);
            uiBackView[Side.Back].Add(back6);
            uiBackView[Side.Back].Add(back7);
            uiBackView[Side.Back].Add(back8);
            uiBackView[Side.Down].Add(down0);
            uiBackView[Side.Down].Add(down1);
            uiBackView[Side.Down].Add(down2);
            uiBackView[Side.Down].Add(down3);
            uiBackView[Side.Down].Add(down4);
            uiBackView[Side.Down].Add(down5);
            uiBackView[Side.Down].Add(down6);
            uiBackView[Side.Down].Add(down7);
            uiBackView[Side.Down].Add(down8);
        }
        private void initializeScrambles()
        {
            scrambles.Add(cbH);
            scrambles.Add(cbUa);
            scrambles.Add(cbUb);
            scrambles.Add(cbZ);

            scrambles.Add(cbV);
            scrambles.Add(cbY);
            scrambles.Add(cbNa);
            scrambles.Add(cbNb);
            scrambles.Add(cbE);

            scrambles.Add(cbGa);
            scrambles.Add(cbGb);
            scrambles.Add(cbGc);
            scrambles.Add(cbGd);
            scrambles.Add(cbAa);
            scrambles.Add(cbAb);
            scrambles.Add(cbT);
            scrambles.Add(cbJa);
            scrambles.Add(cbJb);
            scrambles.Add(cbRa);
            scrambles.Add(cbRb);
            scrambles.Add(cbF);
        }
        private void initializeViews()
        {
            views[cbFace]  = "";   //  0U
            views[cbRight] = "U";  //  1U       
            views[cbBack]  = "U2"; //  2U
            views[cbLeft]  = "U'"; //  1U'
           // views.Add(cbTurnF2L); // not needed, always 0..4 random
        }
        private void initializeTopcolors()
        {
            topColors.Add(cbYellow);
            topColors.Add(cbWhite);
            topColors.Add(cbRed);
            topColors.Add(cbGreen);
            topColors.Add(cbOrange);
            topColors.Add(cbBlue);
        }
        internal List<CheckBox> getSelectedScrambles()
        {
            List<CheckBox> ret = new List<CheckBox>();
            foreach(CheckBox cb in scrambles) {
                if(cb.IsChecked.Value) {
                    ret.Add(cb);
                }
            }//for
            return ret;
        }
        internal List<string/*AUF*/> getSelectedViews()
        {
            var ret = new List<string/*AUF*/>();
            foreach (KeyValuePair<CheckBox, string> pair in views) {
                CheckBox cb = pair.Key;
                if (cb.IsChecked.Value) {
                    ret.Add(pair.Value);
                }
            }//for
            return ret;
        }
        internal List<CheckBox> getSelectedColors()
        {
            List<CheckBox> ret = new List<CheckBox>();
            foreach (CheckBox cb in topColors) {
                if (cb.IsChecked.Value) {
                    ret.Add(cb);
                }
            }//for
            return ret;
        }
        private void updateGui()
        {
            Dictionary<Side, List<Color>> cubeData = theCube.getData();
            foreach (Side side in Enum.GetValues(typeof(Side)))
            {
                switch (side)
                {
                    case Side.Up:
                    case Side.Front:
                    case Side.Right:
                        if (cubeData[side].Count == uiData[side].Count)
                        {
                            for (int counter = 0; counter < cubeData[side].Count; counter++)
                            {
                                uiData[side][counter].Fill = new SolidColorBrush(cubeData[side][counter]);
                            }
                        }
                        break;
                    case Side.Left:
                    case Side.Back:
                    case Side.Down:
                        if (cubeData[side].Count == uiBackView[side].Count)
                        {
                            for (int counter = 0; counter < cubeData[side].Count; counter++)
                            {
                                uiBackView[side][counter].Fill = new SolidColorBrush(cubeData[side][counter]);
                            }
                        }
                        break;
                }//sw
            }//for          
        }
    }
    partial class Practicer {
        bool m_running = false;
        public bool Running
        {
            get { return m_running; }
            set {
                m_running = value;
                form.cbCorrect.IsEnabled = !value;
                form.cbDiagonal.IsEnabled = !value;
                form.cbAdjacent.IsEnabled = !value;
                form.cbView.IsEnabled = !value;
                form.cbTopcolors.IsEnabled = !value;

                form.btStart.IsEnabled = !value;
                form.comPracticeSize.IsEnabled = !value;
                form.tbMovesTodo.IsEnabled = !value;
                form.btReset.IsEnabled = !value;
                form.btExecute.IsEnabled = !value;

                form.cbH.IsEnabled = !value;
                form.cbUa.IsEnabled = !value;
                form.cbUb.IsEnabled = !value;
                form.cbZ.IsEnabled = !value;

                form.cbV.IsEnabled = !value;
                form.cbY.IsEnabled = !value;
                form.cbNa.IsEnabled = !value;
                form.cbNb.IsEnabled = !value;
                form.cbE.IsEnabled = !value;

                form.cbGa.IsEnabled = !value;
                form.cbGb.IsEnabled = !value;
                form.cbGc.IsEnabled = !value;
                form.cbGd.IsEnabled = !value;
                form.cbAa.IsEnabled = !value;
                form.cbAb.IsEnabled = !value;
                form.cbT.IsEnabled = !value;
                form.cbJa.IsEnabled = !value;
                form.cbJb.IsEnabled = !value;
                form.cbRa.IsEnabled = !value;
                form.cbRb.IsEnabled = !value;
                form.cbF.IsEnabled = !value;

                form.cbLeft.IsEnabled = !value;
                form.cbRight.IsEnabled = !value;
                form.cbFace.IsEnabled = !value;
                form.cbBack.IsEnabled = !value;
                form.cbF2Lauf.IsEnabled = !value;

                form.cbYellow.IsEnabled = !value;
                form.cbWhite.IsEnabled = !value;
                form.cbRed.IsEnabled = !value;
                form.cbGreen.IsEnabled = !value;
                form.cbOrange.IsEnabled = !value;
                form.cbBlue.IsEnabled = !value;
            }
        }
        internal string Correct { get; set; }
        MainWindow form; // temporary. must use a delegate
    }
}//ns






/*
        private void Grid_MouseMove(object sender, MouseEventArgs e) {
//            Point p = e.GetPosition(theGrid);
//            this.Title = string.Format("GetPosition(btn1): X = {0}, Y = {1}", p.X, p.Y);
        }

        private int cnt = 0;
        private void theGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
           
            cnt++;
            if (cnt > 4)
            {
                //this.Title = string.Format("({0}, {1}) ", p.X, p.Y);
                cnt = 0;
                this.textBox1.Text = "";
            }
            else
            {
                Point p = e.GetPosition(theGrid);
                if(this.textBox1.Text.Length != 0) {
                    this.textBox1.Text += ", ";
                }
                this.textBox1.Text += string.Format("{0} {1}", p.X, p.Y);
            }
        }
        */
