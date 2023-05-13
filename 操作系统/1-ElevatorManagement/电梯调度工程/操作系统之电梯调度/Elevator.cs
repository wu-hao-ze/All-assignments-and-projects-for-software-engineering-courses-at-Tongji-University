using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 操作系统之电梯调度
{
    public class Elevator
    {
        public enum ELEVATORSTATE
        {
            MOVING, DOOROPEN, DOORCLOSE, DOOROPENING, DOORCLOSING
        }

        public ELEVATORSTATE elevatorState;//电梯是否处于运行状态

        public bool eleveatorWarning;//电梯是否处于警告状态

        //电梯整体运动状态
        public enum ELEVATORMOVINGSTATE
        {
            UP, DOWN, WAIT
        }
        public ELEVATORMOVINGSTATE elevatorMovingState;

        public enum DOORSTATE
        {
            OPEN, OPEN_NEAR, CLOSE_NEAR, CLOSE
        };
        public DOORSTATE doorState;

        public int floor;//当前楼层
        private PictureBox[] floors;
        private int maxFloor;//当前所在的最大楼层

        public PictureBox image;
        private Timer doorTimer; //开关门计时器
        private Timer movingTimer;//移动计时器
        private Timer globalTimer;//全局计时器

        private int movingState; //移动一层楼需要30次，因此需要该变量
        private int movingTime;//当前已移动楼层数

        //开门等待计数器
        private int doorOpenCounter;

        //位置信息
        private Point location; //电梯所在位置
        private Point buildingLocation;//建筑所在位置

        //楼层显示
        private Label floorLabel;

        //需要暂停的楼层编号
        public bool[] stopFloor;

        //绑定外部按钮
        public List<Button> outsideButton;
        //绑定内部按钮
        public List<Button> insideButton;

        //电梯移动状态标识
        PictureBox movingNoticeImage;
        Timer movingNoticeTimer;
        int movingNotice;

        //图像大小
        private int maxSize;

        //父控件的滚轮
        Form parent;

        //上一次滚轮的位置
        Point lastPosition;

        //提醒消息
        ToolTip elevatorInfo;

        //当前人数
        public int personNum;

        //构造函数
        public Elevator(Form parent, int maxFloor, Point buildingLocation,
            int maxSize, FontFamily FML)
        {
            this.parent = parent;
            this.maxSize = maxSize;

            //电梯状态的初始化
            this.elevatorState = ELEVATORSTATE.DOORCLOSE;
            this.doorState = DOORSTATE.CLOSE;
            this.elevatorMovingState = ELEVATORMOVINGSTATE.WAIT;
            this.eleveatorWarning = false;

            //当前人数
            this.personNum = 0;

            //楼层的初始化
            this.floor = 1;
            this.maxFloor = maxFloor;

            //上次位置
            lastPosition = new Point(0, 0);

            //开/关 门计时器
            this.doorTimer = new Timer();
            this.doorTimer.Tick += this.doorTimerTick;
            this.doorTimer.Interval = 500;

            //全局计时器
            this.globalTimer = new Timer
            {
                Interval = 1
            };
            this.globalTimer.Tick += this.globalTimerTick;
            this.globalTimer.Start();

            //移动计时器
            this.movingTimer = new Timer
            {
                Interval = 15
            };
            this.movingTimer.Tick += this.movingTimerTick;
            this.movingState = 0;

            Size floorSize = new Size(maxSize, maxSize);

            //电梯楼层显示
            floorLabel = new Label
            { 
                Text = "01",
                Parent = parent,
                Location = new Point(buildingLocation.X, buildingLocation.Y),
                Size = new Size(maxSize * 2, maxSize)
            };

            floorLabel.Font = new Font(FML, 15);
            floorLabel.BringToFront();

            //电梯移动标识的初始化
            this.movingNotice = 0;
            movingNoticeImage = new PictureBox
            {
                Parent = parent,
                Location = new Point(buildingLocation.X + floorSize.Width + 30, buildingLocation.Y),
                Size = floorSize
            };
            this.movingNoticeImage.BringToFront();
            this.movingNoticeImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.movingNoticeImage.Image = Image.FromFile("img//停止.png");

            this.movingNoticeTimer = new Timer
            {
                Interval = 250
            };
            this.movingNoticeTimer.Tick += movingNoticeTimerTick;
            this.movingNoticeTimer.Start();

            buildingLocation.Y = buildingLocation.Y + floorSize.Height + 10;

            //建筑初始化
            this.buildingLocation = buildingLocation;
            //根据楼层数生成楼层
            this.floors = new PictureBox[maxFloor];

            Point floorLocation = new Point(buildingLocation.X,
                buildingLocation.Y);

            Size floorImageSize = new Size(maxSize, maxSize + 10);
            for (int i = 0; i < maxFloor; ++i)
            {
                if (i != 0)
                {
                    floorLocation.Y += maxSize;
                }
                floors[i] = new PictureBox
                {
                    Location = floorLocation,
                    Parent = parent
                };

                if (i == maxFloor - 1)
                {
                    floors[i].Size = floorSize;
                }
                else
                {
                    floors[i].Size = floorImageSize;
                }
                floors[i].Image = Image.FromFile("img//楼层.png");
            }

            //图像初始化
            this.image = new PictureBox
            {
                Image = Image.FromFile("img//电梯-close.png"),
                Parent = parent,
                Location = floorLocation,
                Size = floorSize,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.image.BringToFront();

            elevatorInfo = new ToolTip
            {
                IsBalloon = true
            };
            updateInfo();

            this.location = this.image.Location;

            //需要暂停的楼层编号
            this.stopFloor = new bool[maxFloor + 1];
            for (int i = 1; i <= maxFloor; ++i)
            {
                this.stopFloor[i] = false;
            }
            this.outsideButton = new List<Button>();
            this.insideButton = new List<Button>();

            //开门等待计数器
            this.doorOpenCounter = 0;

        }

        //电梯开门
        public bool doorOpen()
        {
            this.elevatorState = ELEVATORSTATE.DOOROPENING;
            this.doorTimer.Start();

            return true;
        }

        //电梯关门
        public bool doorClose()
        {
            if (this.elevatorState == ELEVATORSTATE.DOORCLOSE)
            {
                return true;
            }
            this.elevatorState = ELEVATORSTATE.DOORCLOSING;
            this.doorTimer.Start();

            return true;
        }

        //电梯向上移动
        public bool goUp()
        {
            //查看当前状态是否允许上移
            if (this.elevatorState != ELEVATORSTATE.DOORCLOSE
                || this.floor == this.maxFloor)
            {
                return false;
            }

            //打开门
            this.elevatorState = ELEVATORSTATE.MOVING;
            this.elevatorMovingState = ELEVATORMOVINGSTATE.UP;
            this.movingTimer.Start();
            this.movingTime = 0;

            return true;
        }

        //电梯向下移动
        public bool goDown()
        {
            //查看当前状态是否允许下移
            if (this.elevatorState != ELEVATORSTATE.DOORCLOSE
                || this.floor == 1)
            {
                return false;
            }

            //打开门
            this.elevatorState = ELEVATORSTATE.MOVING;
            this.elevatorMovingState = ELEVATORMOVINGSTATE.DOWN;
            this.movingTimer.Start();
            this.movingTime = 0;

            return true;
        }

        private int abs(int x)
        {
            if (x < 0)
                return -x;
            else
                return x;
        }

        //按钮任务完成处理
        private void buttonEndDeal()
        {
            //当门关闭后，处理外部按钮
            for (int i = 0; i < outsideButton.Count(); ++i)
            {
                if (int.Parse(this.outsideButton[i].Name.Substring(1)) == floor)
                {
                    outsideButton[i].Enabled = true;
                    if (outsideButton[i].Name[0] == '+')
                    {
                        outsideButton[i].BackgroundImage = Image.FromFile("img//向上.png");
                    }
                    else
                    {
                        outsideButton[i].BackgroundImage = Image.FromFile("img//向下.png");
                    }
                    outsideButton.RemoveAt(i);
                    break;
                }
            }

            //当门关闭后，处理内部按钮
            for (int i = 0; i < insideButton.Count(); ++i)
            {
                if (int.Parse(this.insideButton[i].Name.Substring(1)) == this.floor)
                {
                    insideButton[i].Enabled = true;
                    insideButton.RemoveAt(i);
                    break;
                }
            }
        }
        private void globalTimerTick(object sender, EventArgs e)
        {
            for (int i = 0; i < outsideButton.Count(); ++i)
            {
                if (outsideButton[i].Enabled)
                {
                    outsideButton.RemoveAt(i);
                    break;
                }
            }

            //如果电梯处于警戒状态
            if (this.eleveatorWarning)
            {
                if (this.elevatorState == ELEVATORSTATE.DOORCLOSE ||
                    this.elevatorState == ELEVATORSTATE.DOOROPENING)
                {
                    //强制开门，并开始计时
                    this.doorOpen();
                }

                return;
            }

            if (this.elevatorMovingState == ELEVATORMOVINGSTATE.UP)
            {
                //如果电梯状态也为DOORCLOSE，那么我们就可以向上走了
                if (this.elevatorState == ELEVATORSTATE.DOORCLOSE)
                {
                    buttonEndDeal();

                    //向上移动终止条件：所有上方任务完成
                    bool changeState = true;
                    for (int i = this.floor + 1; i <= this.maxFloor; ++i)
                    {
                        if (this.stopFloor[i])
                        {
                            changeState = false;
                            break;
                        }
                    }

                    if (changeState)
                    {
                        this.elevatorMovingState = ELEVATORMOVINGSTATE.WAIT;
                        return;
                    }

                    //调用向上移动函数
                    this.goUp();
                }
                else if (this.elevatorState == ELEVATORSTATE.DOOROPEN)
                {
                    //如果门已经被打开了，那么需要等待2s门关闭
                    if (this.doorOpenCounter == 0)
                    {
                        this.doorOpenCounter = 90;
                    }
                    else if (this.doorOpenCounter > 0)
                    {
                        this.doorOpenCounter--;
                        //进入关门事件
                        if (this.doorOpenCounter == 0)
                        {
                            this.elevatorState = ELEVATORSTATE.DOORCLOSING;
                            this.doorClose();
                        }
                    }
                }
                else if (this.elevatorState == ELEVATORSTATE.DOOROPENING)
                {
                    this.stopFloor[this.floor] = false;

                    //调用开关门函数
                    this.doorOpen();
                }
            }
            else if (this.elevatorMovingState == ELEVATORMOVINGSTATE.DOWN)
            {
                if (this.elevatorState == ELEVATORSTATE.DOORCLOSE)
                {
                    buttonEndDeal();

                    //向下移动终止条件：所有下方任务完成
                    bool changeState = true;
                    for (int i = 1; i < this.floor; ++i)
                    {
                        if (this.stopFloor[i])
                        {
                            changeState = false;
                            break;
                        }
                    }

                    if (changeState)
                    {
                        this.elevatorMovingState = ELEVATORMOVINGSTATE.WAIT;
                        return;
                    }

                    //调用向上移动函数
                    this.goDown();
                }
                else if (this.elevatorState == ELEVATORSTATE.DOOROPEN)
                {
                    //如果门已经被打开了，那么需要等待2s门关闭
                    if (this.doorOpenCounter == 0)
                    {
                        this.doorOpenCounter = 90;
                    }
                    else if (this.doorOpenCounter > 0)
                    {
                        this.doorOpenCounter--;
                        //进入关门事件
                        if (this.doorOpenCounter == 0)
                        {
                            this.elevatorState = ELEVATORSTATE.DOORCLOSING;
                            this.doorClose();
                        }
                    }
                }
                else if (this.elevatorState == ELEVATORSTATE.DOOROPENING)
                {
                    this.stopFloor[this.floor] = false;


                    //调用开关门函数
                    this.doorOpen();
                }
            }
            else
            {
                //如果电梯门为打开状态
                if (this.elevatorState == ELEVATORSTATE.DOOROPEN)
                {
                    //如果门已经被打开了，那么需要等待2s门关闭
                    if (this.doorOpenCounter == 0)
                    {
                        this.doorOpenCounter = 90;
                    }
                    else if (this.doorOpenCounter > 0)
                    {
                        this.doorOpenCounter--;
                        //进入关门事件
                        if (this.doorOpenCounter == 0)
                        {
                            this.elevatorState = ELEVATORSTATE.DOORCLOSING;
                            this.doorClose();
                        }
                    }
                }
                else if (this.elevatorState == ELEVATORSTATE.DOORCLOSE)
                {

                    buttonEndDeal();

                    //查看是否有外部调度尚未执行
                    if (outsideButton.Count() != 0)
                    {
                        //优先前往距离最近的楼层
                        outsideButton.Sort((x, y) => (
                        abs(int.Parse(x.Name.Substring(1)) - this.floor) -
                        abs(int.Parse(y.Name.Substring(1)) - this.floor)));

                        int targetFloor = int.Parse(outsideButton[0].Name.Substring(1));
                        if (targetFloor > this.floor)
                        {

                            this.elevatorMovingState = ELEVATORMOVINGSTATE.UP;
                        }
                        else if (targetFloor < this.floor)
                        {

                            this.elevatorMovingState = ELEVATORMOVINGSTATE.DOWN;
                        }
                        return;
                    }

                    //查看是否有需要前往的楼层
                    if (insideButton.Count() != 0)
                    {
                        insideButton.Sort((x, y) => (
                        abs(int.Parse(x.Name.Substring(1)) - this.floor) -
                        abs(int.Parse(y.Name.Substring(1)) - this.floor)));
                        //优先前往第一个的位置
                        int targetFloor = int.Parse(insideButton[0].Name.Split()[1]);
                        if (targetFloor > this.floor)
                        {

                            this.elevatorMovingState = ELEVATORMOVINGSTATE.UP;
                        }
                        else if (targetFloor < this.floor)
                        {

                            this.elevatorMovingState = ELEVATORMOVINGSTATE.DOWN;
                        }
                        return;
                    }
                }
                else if (this.elevatorState == ELEVATORSTATE.DOOROPENING)
                {
                    this.doorOpen();
                }

            }
        }

        private void doorTimerTick(object sender, EventArgs e)
        {
            //关门事件，每一秒钟调用一次电梯门状态的改变
            if (this.elevatorState == ELEVATORSTATE.DOORCLOSING)
            {
                switch (this.doorState)
                {
                    case Elevator.DOORSTATE.OPEN:
                        this.doorState = Elevator.DOORSTATE.OPEN_NEAR;
                        if (personNum > 0)
                        {
                            this.image.Image = Image.FromFile("img//电梯-open-near-人.png");
                        }
                        else
                        {
                            this.image.Image = Image.FromFile("img//电梯-open-near.png");
                        }
                        break;
                    case Elevator.DOORSTATE.OPEN_NEAR:
                        this.doorState = Elevator.DOORSTATE.CLOSE_NEAR;
                        if (personNum > 0)
                        {
                            this.image.Image = Image.FromFile("img//电梯-close-near-人.png");
                        }
                        else
                        {
                            this.image.Image = Image.FromFile("img//电梯-close-near.png");
                        }
                        break;
                    case Elevator.DOORSTATE.CLOSE_NEAR:
                        this.doorState = Elevator.DOORSTATE.CLOSE;
                        this.elevatorState = Elevator.ELEVATORSTATE.DOORCLOSE;
                        this.image.Image = Image.FromFile("img//电梯-close.png");
                        this.doorTimer.Stop();
                        break;
                }
            }
            //电梯开门
            else if (this.elevatorState == ELEVATORSTATE.DOOROPENING)
            {
                switch (this.doorState)
                {
                    case Elevator.DOORSTATE.CLOSE:
                        this.doorState = Elevator.DOORSTATE.CLOSE_NEAR;
                        if (personNum > 0)
                        {
                            this.image.Image = Image.FromFile("img//电梯-close-near-人.png");
                        }
                        else
                        {
                            this.image.Image = Image.FromFile("img//电梯-close-near.png");
                        }
                        break;
                    case Elevator.DOORSTATE.CLOSE_NEAR:
                        this.doorState = Elevator.DOORSTATE.OPEN_NEAR;
                        if (personNum > 0)
                        {
                            this.image.Image = Image.FromFile("img//电梯-open-near-人.png");
                        }
                        else
                        {
                            this.image.Image = Image.FromFile("img//电梯-open-near.png");
                        }
                        break;
                    case Elevator.DOORSTATE.OPEN_NEAR:
                        this.doorState = Elevator.DOORSTATE.OPEN;
                        this.elevatorState = Elevator.ELEVATORSTATE.DOOROPEN;
                        if (personNum > 0)
                        {
                            this.image.Image = Image.FromFile("img//电梯-open-人.png");
                        }
                        else
                        {
                            this.image.Image = Image.FromFile("img//电梯-open.png");
                        }
                        this.doorTimer.Stop();
                        break;
                }
            }
        }

        private void movingTimerTick(object sender, EventArgs e)
        {
            //如果当前状态为向上移动,则Y坐标+1
            if (this.elevatorMovingState == ELEVATORMOVINGSTATE.UP)
            {
                //this.location.Y--;
                this.location.Y = this.location.Y - lastPosition.Y + parent.AutoScrollPosition.Y - 1;
            }
            else if (this.elevatorMovingState == ELEVATORMOVINGSTATE.DOWN)
            {
                //this.location.Y++;
                this.location.Y = this.location.Y - lastPosition.Y + parent.AutoScrollPosition.Y + 1;
            }
            this.location.X = this.location.X - lastPosition.X + parent.AutoScrollPosition.X;

            //更新滚轮位置
            lastPosition.X = parent.AutoScrollPosition.X;
            lastPosition.Y = parent.AutoScrollPosition.Y;

            this.image.Location = this.location;
            ++this.movingState;

            if (movingState >= maxSize)
            {
                ++this.movingTime;
                this.movingState = 0;
                //一个楼层已经移动完毕
                if (this.elevatorMovingState == ELEVATORMOVINGSTATE.UP)
                {
                    this.floor++;
                }
                else if (this.elevatorMovingState == ELEVATORMOVINGSTATE.DOWN)
                {
                    this.floor--;
                }

                //楼层标识
                if (this.floor < 10)
                {
                    this.floorLabel.Text = "0" + (this.floor).ToString();
                }
                else
                {
                    this.floorLabel.Text = (this.floor).ToString();
                }

                //向上移动终止条件
                //1.本层楼需要暂停(当前运行方向和外部指令方向相同)
                //2.所有上面的命令执行完毕
                //3.已经进入警戒状态
                this.updateInfo();
                this.movingTime = 0;

                if (this.stopFloor[this.floor] || this.eleveatorWarning)
                {
                    this.movingTimer.Stop();
                    this.elevatorState = ELEVATORSTATE.DOOROPENING;
                }
            }
        }

        //标识电梯上升下降状态的计时器
        private void movingNoticeTimerTick(object sender, EventArgs e)
        {
            //警报状态
            if (this.eleveatorWarning)
            {
                switch (movingNotice)
                {
                    case 0:
                        movingNotice++;
                        this.movingNoticeImage.Image = Image.FromFile("img//警报1.png");
                        break;
                    default:
                        movingNotice = 0;
                        this.movingNoticeImage.Image = Image.FromFile("img//警报2.png");
                        break;
                }
                return;
            }
            if (this.elevatorMovingState == ELEVATORMOVINGSTATE.UP)
            {
                switch (movingNotice)
                {
                    case 0:
                        movingNotice++;
                        this.movingNoticeImage.Image = Image.FromFile("img//上升1.png");
                        break;
                    case 1:
                        movingNotice++;
                        this.movingNoticeImage.Image = Image.FromFile("img//上升2.png");
                        break;
                    case 2:
                        movingNotice++;
                        this.movingNoticeImage.Image = Image.FromFile("img//上升3.png");
                        break;
                    case 3:
                        movingNotice = 0;
                        this.movingNoticeImage.Image = Image.FromFile("img//上升4.png");
                        break;
                }
            }
            else if (this.elevatorMovingState == ELEVATORMOVINGSTATE.DOWN)
            {
                switch (movingNotice)
                {
                    case 0:
                        movingNotice++;
                        this.movingNoticeImage.Image = Image.FromFile("img//下降1.png");
                        break;
                    case 1:
                        movingNotice++;
                        this.movingNoticeImage.Image = Image.FromFile("img//下降2.png");
                        break;
                    case 2:
                        movingNotice++;
                        this.movingNoticeImage.Image = Image.FromFile("img//下降3.png");
                        break;
                    case 3:
                        movingNotice = 0;
                        this.movingNoticeImage.Image = Image.FromFile("img//下降4.png");
                        break;
                }
            }
            else
            {
                movingNoticeImage.Image = Image.FromFile("img//停止.png");
            }

        }

        //警告键
        public void warningState()
        {
            //警戒状态更改
            this.eleveatorWarning = !this.eleveatorWarning;
            if (!eleveatorWarning)
            {
                this.doorClose();
            }
        }

        public void updateInfo()
        {
            elevatorInfo.SetToolTip(this.image,
                "当前楼层: " + floor.ToString() + "\n当前人数: " + personNum.ToString());
        }

    }
}
