using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameApp
{
    public partial class Form1 : Form
    {
        #region fields
        Maze maze;
        PlayerControl player;
        Treasures treasures;
        SoundPlayer soundPlayer;
        DateTime startTime;
        List<Keys> keyDown = new List<Keys>();
        Dictionary<Keys, Point> move = new Dictionary<Keys, Point>()
        {
            { Keys.Up,new Point(0,-1) },
            { Keys.Down,new Point(0,1) },
            { Keys.Left,new Point(-1,0) },
            { Keys.Right,new Point(1,0) },
        };
        #endregion fields

        #region ctor
        public Form1()
        {
            InitializeComponent();
            startTime = DateTime.Now;
            soundPlayer = new SoundPlayer("tada.wav");

            MazeInit();
            PlayerInit();
            TreasuresInit(15);
            
            this.BackColor = Color.White;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            this.ControlRemoved += Form1_ControlRemoved;
        }
        #endregion ctor

        #region Initializers
        private void MazeInit()
        {
            maze = new Maze(this.ClientRectangle);
            maze.RecursiveDivideRect(this.ClientRectangle);
        }
        private void PlayerInit()
        {
            Rectangle playerStartArea = new Rectangle(
                this.ClientRectangle.Right / 3,
                this.ClientRectangle.Bottom / 3,
                this.ClientRectangle.Width / 3,
                this.ClientRectangle.Height / 3
                );
            player = new PlayerControl();
            player.Location = maze.GetLocation(player.Size, playerStartArea);
            Controls.Add(player);
        }
        private void TreasuresInit(int count)
        {
            treasures = new Treasures(count);
            foreach (var tr in treasures)
                tr.Location = maze.GetLocation(tr.Size, this.ClientRectangle);
            Controls.AddRange(treasures.ToArray());
        }
        #endregion Initializers

        private void Form1_ControlRemoved(object sender, ControlEventArgs e)
        {
            treasures.Remove(e.Control as PictureBox);
            soundPlayer.Play();
            if (treasures.Count() == 0)
                EndGame();
        }
        #region key_events
        private void Form1_KeyUp(object sender, KeyEventArgs e)
            => keyDown.RemoveAll(key => key == e.KeyCode);
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(keyDown.FindAll(key=> key == e.KeyCode).Count < 3) //Acceleration
                keyDown.Add(e.KeyCode);
            Point movement;
            keyDown.ForEach(key =>
            {
                if (move.TryGetValue(key, out movement))
                {
                    Rectangle moveTo = MovePlayer(movement);
                    PlayerMoved(moveTo);
                }
            });
        }
        #endregion key_events
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillPath(Brushes.Black,this.maze.Path);
        }
        private Rectangle MovePlayer(Point movement)
        {
            Rectangle moveTo = new Rectangle(player.Location, player.Size);
            moveTo.Offset(movement);
            if (maze.isClear(moveTo))
                player.updateLocation(movement);
            return moveTo;
        }
        private void PlayerMoved(Rectangle moveTo)
        {
            if (treasures.isTaken(moveTo))
            {
                PictureBox x = treasures.First(tr => tr.Bounds.IntersectsWith(moveTo));
                Controls.Remove(x);
            }
        }

        private void EndGame()
        {
            TimeSpan interval = DateTime.Now.Subtract(startTime);
            string time = $"{interval.Minutes}:{interval.Seconds}";
            MessageBox.Show($"Completion time {time}");

        }

    }
}
