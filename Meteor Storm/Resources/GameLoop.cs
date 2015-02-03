using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Meteor_Storm
{
    public class GameLoop
    {
        protected DateTime _lastTick;
        public delegate void UpdateHandler(TimeSpan elapsed);
        public event UpdateHandler Update;
        private Storyboard gameLoop;
        protected bool stopped;

        public GameLoop(FrameworkElement parent)
            : this(parent, 0)
        {

        }

        public GameLoop(FrameworkElement parent, double storyBoardMilliseconds)
        {
            gameLoop = new Storyboard();
            gameLoop.Duration = TimeSpan.FromMilliseconds(storyBoardMilliseconds);
            gameLoop.SetValue(FrameworkElement.NameProperty, "gameloop");
            parent.Resources.Add("gameloop", gameLoop);
            gameLoop.Completed += new EventHandler(gameLoop_Completed);
        }

        void gameLoop_Completed(object sender, EventArgs e)
        {
            if (stopped) return;
            Tick();
            (sender as Storyboard).Begin();
        }

        public void Tick()
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - _lastTick;
            _lastTick = now;
            if (Update != null) Update(elapsed);
        }

        public void Start()
        {
            stopped = false;
            if (gameLoop != null)
            {
                gameLoop.Begin();
            }
            _lastTick = DateTime.Now;
        }

        public void Stop()
        {
            stopped = true;
        }
    }
}
