using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 操作系统之电梯调度
{
    public class HumanBeing
    {
        public int startFloor;
        public int endFloor;
        public int selectedElevator;
        public bool finished;
        public Timer personTimer;
        public int maxHeight;
        public Form1 parent;
        public enum STATE
        {
            NOT, WAITING, TAKING
        }
        public STATE state;
        public HumanBeing(int maxHeight, Form1 parent)
        {
            this.maxHeight = maxHeight;
            this.parent = parent;
            state = STATE.NOT;
            selectedElevator = -1;
            finished = false;
            Random randoms = new Random();
            personTimer = new Timer()
            {
                Interval = randoms.Next(2000, 4000)//随机到达时间
            };
            personTimer.Tick += timeTicker;
            personTimer.Start();
        }
        private void timeTicker(object sender, EventArgs e)
        {
            if (state == STATE.NOT)
            {
                personTimer.Stop();
                //随机产生楼层
                Random randoms = new Random();
                startFloor = randoms.Next(1, maxHeight);
                //如果该开始楼层有人，则再等一个轮回

                do
                {
                    endFloor = randoms.Next(1, maxHeight);
                } while (startFloor == endFloor);

                parent.waitingHuman[maxHeight - startFloor]++;
                parent.waitingHumanImage[maxHeight - startFloor].Image = Image.FromFile("img//等待.png");
                parent.elevatorLog("乘客到达" + startFloor + "楼，需要前往" + endFloor + "楼");
                state = STATE.WAITING;
                personTimer.Interval = 1;
                personTimer.Start();
                //乘客按下相应按钮
                if (endFloor > startFloor)
                {
                    Button findButton = (Button)parent.outsideControl.Controls.Find("+" + startFloor, false)[0];
                    findButton.PerformClick();
                }
                else
                {
                    Button findButton = (Button)parent.outsideControl.Controls.Find("-" + startFloor, false)[0];
                    findButton.PerformClick();
                }

            }
            else if (state == STATE.WAITING)
            {
                //查看是否有电梯到达本层并且开门了
                for (int i = 0; i < parent.elevator.Length; ++i)
                {
                    if (parent.elevator[i].elevator.floor == startFloor &&
                        parent.elevator[i].elevator.doorState == Elevator.DOORSTATE.OPEN)
                    {
                        //进入电梯
                        parent.elevator[i].elevator.personNum++;
                        parent.elevator[i].elevator.image.Image = Image.FromFile("img//电梯-open-人.png");
                        parent.elevator[i].elevator.updateInfo();
                        state = STATE.TAKING;
                        selectedElevator = i;
                        parent.waitingHuman[maxHeight - startFloor]--;
                        if (parent.waitingHuman[maxHeight - startFloor] == 0)
                        {
                            parent.waitingHumanImage[maxHeight - startFloor].Image = Image.FromFile("img//无人等待.png");
                        }
                        //按下楼层按键
                        parent.elevator[i].floorButton[maxHeight - endFloor].PerformClick();
                        break;
                    }
                }
            }
            else if (state == STATE.TAKING)
            {
                //电梯到达对应楼层
                if (parent.elevator[selectedElevator].elevator.floor == endFloor &&
                    parent.elevator[selectedElevator].elevator.doorState == Elevator.DOORSTATE.OPEN)
                {
                    personTimer.Stop();
                    //离开电梯
                    parent.elevator[selectedElevator].elevator.personNum--;
                    if (parent.elevator[selectedElevator].elevator.personNum == 0)
                    {
                        parent.elevator[selectedElevator].elevator.image.Image = Image.FromFile("img//电梯-open.png");
                    }
                    parent.elevator[selectedElevator].elevator.updateInfo();
                    parent.elevatorLog("乘客到达目标楼层，离开电梯");
                    startFloor = -1;
                    endFloor = -1;
                    selectedElevator = -1;
                    Random randoms = new Random();
                    personTimer.Interval = randoms.Next(2000, 4000);//随机到达时间
                    personTimer.Start();
                    state = STATE.NOT;
                }
                //所乘的电梯进入了警报状态，并且门已经被打开，则离开
                else if (parent.elevator[selectedElevator].elevator.eleveatorWarning)
                {
                    //离开电梯
                    personTimer.Stop();
                    parent.elevator[selectedElevator].elevator.personNum--;
                    if (parent.elevator[selectedElevator].elevator.personNum == 0)
                    {
                        parent.elevator[selectedElevator].elevator.image.Image = Image.FromFile("img//电梯-open.png");
                    }
                    parent.elevator[selectedElevator].elevator.updateInfo();
                    parent.elevatorLog("电梯处于警报状态，乘客离开电梯");
                    startFloor = -1;
                    endFloor = -1;
                    selectedElevator = -1;
                    Random randoms = new Random();
                    personTimer.Interval = randoms.Next(2000, 4000);//随机到达时间
                    personTimer.Start();
                    state = STATE.NOT;
                }
            }
        }
    }

    public partial class Form1 : Form
    {

        private int elevatorNum;
        private int buildingHeight;
        private int maxSize;
        public ElevatorControl[] elevator;
        private List<ElevatorRequest> requests;
        private RichTextBox richTextBox;
        private NumericUpDown elevatorNumeric;
        private NumericUpDown heightNumeric;
        private NumericUpDown personNumeric;
        private List<HumanBeing> humanBeings;
        public PictureBox[] waitingHumanImage;
        public GroupBox outsideControl;
        public int[] waitingHuman;
        private Timer algorithmTimer;
        private FontFamily FML;
        public class ElevatorControl
        {
            public Elevator elevator;
            public Button[] floorButton;
            public Button warningButton;
        };

        private class ElevatorRequest
        {
            //起始楼层
            public int startFloor;
            //是否为上楼请求
            public bool up;
            //正在处理该请求的电梯
            public int elevatorIndex;
            //产生该请求的按钮
            public Button requestButton;

            //构造函数
            public ElevatorRequest(int startFloor, bool up, Button requestButton)
            {
                this.startFloor = startFloor;
                this.up = up;
                elevatorIndex = -1;
                this.requestButton = requestButton;
            }
        }

        public Form1(int elevatorNum = 5, int buildingHeight = 20)
        {
            this.elevatorNum = elevatorNum;
            this.buildingHeight = buildingHeight;

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //消息栏
            this.richTextBox = new RichTextBox
            {
                HideSelection = false,
                EnableAutoDragDrop = false
            };
            //this.contextMenuStrip1.Items[0].Click += clearInformation;

            //读取字体
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile("img//digifaw.ttf");
            this.FML = pfc.Families[0];

            //电梯初始化
            richTextBox.AppendText("◎  " + "电梯调度系统初始化，共" + buildingHeight + "层" + elevatorNum + "部电梯");

            this.maxSize = 25;
            elevator = new ElevatorControl[elevatorNum];

            //起始位置
            Point position = new Point(120, 100);
            Size floorButtonSize = new Size(60, maxSize - 5);

            for (int i = 0; i < elevatorNum; ++i)
            {
                elevator[i] = new ElevatorControl
                {
                    elevator = new Elevator(this, buildingHeight, position, maxSize,FML),
                    floorButton = new Button[buildingHeight]
                };

                for (int j = 0; j < buildingHeight; ++j)
                {
                    elevator[i].floorButton[j] = new Button
                    {
                        Size = floorButtonSize,
                        //楼层数
                        Text = (buildingHeight - j).ToString(),
                        Name = (i).ToString() + " " + (buildingHeight - j).ToString(),
                        Parent = this,
                        Location =
                        new Point(position.X + maxSize + 10, position.Y + floorButtonSize.Height + 10 + maxSize * j + maxSize / 10),

                    };

                    //内部按钮事件
                    elevator[i].floorButton[j].Click += insideRequest;
                }

                //警告按钮
                elevator[i].warningButton = new Button
                {
                    Size = floorButtonSize,
                    Text = "警报",
                    Name = (i).ToString(),
                    Parent = this,
                    Location =
                    new Point(position.X + maxSize + 10, position.Y + floorButtonSize.Height + 10 + maxSize * buildingHeight + maxSize / 10)
                };
                elevator[i].warningButton.Click += warningRequest;

                //开门按钮
                Size smallButtonSize = new Size(30, maxSize - 5);
                elevator[i].warningButton = new Button
                {
                    Size = smallButtonSize,
                    Text = "开",
                    Name = (i).ToString(),
                    Parent = this,
                    Location =
                    new Point(position.X, position.Y + floorButtonSize.Height + 10 + maxSize * (buildingHeight + 1) + maxSize / 10)
                };

                elevator[i].warningButton.Click += openRequest;

                elevator[i].warningButton = new Button
                {
                    Size = smallButtonSize,
                    Text = "关",
                    Name = (i).ToString(),
                    Parent = this,
                    Location =
                    new Point(position.X + 40, position.Y + floorButtonSize.Height + 10 + maxSize * (buildingHeight + 1) + maxSize / 10)
                };

                elevator[i].warningButton.Click += closeRequest;

                position.X += 120;
            }

            //设置外部按钮
            outsideControl = new GroupBox
            {
                Text = "外部控制",
                Parent = this,
                Name = "外部控制",
                Size = new Size(160, maxSize * (buildingHeight + 2)),
                Location = position
            };

            //请求
            requests = new List<ElevatorRequest>();

            //在最右侧增加控制按钮
            for (int j = 0; j < buildingHeight; ++j)
            {
                //标签
                Label floorLabel = new Label
                {
                    Text = "F" + (buildingHeight - j).ToString(),
                    Location = new Point(10, maxSize * j + 5 + floorButtonSize.Height + 10),
                    AutoSize = true
                };
                outsideControl.Controls.Add(floorLabel);

                //up
                if (j != 0)
                {
                    Button upButton = new Button
                    {
                        Size = new Size(24, 24),
                        Text = "",
                        Name = "+" + (buildingHeight - j).ToString(),
                        Location = new Point(60, maxSize * j + 15 + floorButtonSize.Height),
                        BackgroundImage = Image.FromFile("img//向上.png"),
                        FlatStyle = FlatStyle.Flat,
                        BackgroundImageLayout = ImageLayout.Stretch
                    };
                    upButton.FlatAppearance.BorderSize = 0;
                    upButton.FlatAppearance.MouseOverBackColor = BackColor;

                    outsideControl.Controls.Add(upButton);
                    upButton.Click += outsideRequest;
                }

                //down
                if (j != buildingHeight - 1)
                {
                    Button downButton = new Button
                    {
                        Size = new Size(24, 24),
                        Text = "",
                        Name = "-" + (buildingHeight - j).ToString(),
                        Location = new Point(100, maxSize * j + 15 + floorButtonSize.Height),
                        BackgroundImage = Image.FromFile("img//向下.png"),
                        FlatStyle = FlatStyle.Flat,
                        BackgroundImageLayout = ImageLayout.Stretch
                    };
                    downButton.FlatAppearance.BorderSize = 0;
                    downButton.FlatAppearance.MouseOverBackColor = BackColor;

                    downButton.BringToFront();
                    outsideControl.Controls.Add(downButton);
                    downButton.Click += outsideRequest;
                }
            }

            //文本框
            richTextBox.Size = new Size(position.X - 50, 60);
            richTextBox.Parent = this;
            richTextBox.Location = new Point(50, 30);

            //调度算法执行频率
            algorithmTimer=new Timer();
            algorithmTimer.Interval = 1;
            algorithmTimer.Tick+=timer1_Tick;
            algorithmTimer.Start();

            //数量控制
            GroupBox parameterControl = new GroupBox()
            {
                Text = "参数控制",
                Parent = this,
                Location = new Point(position.X, 30),
                Size = new Size(320, 70)
            };

            Label elevatorNumLabel = new Label()
            {
                Text = "电梯数量",
                Parent = parameterControl,
                Location = new Point(15, 30),
                AutoSize = true
            };

            elevatorNumeric = new NumericUpDown
            {
                Value = elevatorNum,
                Location = new Point(90, 25),
                Minimum = 1,
                Size = new Size(50, 50),
                Parent = parameterControl
            };

            Label buildingHeightLabel = new Label()
            {
                Text = "高度",
                Location = new Point(145, 30),
                AutoSize = true,
                Parent = parameterControl
            };

            heightNumeric = new NumericUpDown
            {
                Value = buildingHeight,
                Parent = parameterControl,
                Location = new Point(185, 25),
                Minimum = 2,
                Size = new Size(50, 50)
            };

            Button resetButton = new Button()
            {
                Text = "修改",
                Location = new Point(240, 25),
                Parent = parameterControl,
                AutoSize = true
            };
            resetButton.Click += restart;

            //自动控制组件
            GroupBox autoControlGroupBox = new GroupBox()
            {
                Text = "自动控制",
                Size = new Size(275, 70),
                Location = new Point(position.X, position.Y + floorButtonSize.Height + maxSize * (buildingHeight + 1) + 10),
                Parent = this
            };
            autoControlGroupBox.BringToFront();
            Label personLabel = new Label()
            {
                Text = "当前乘电梯人数",
                Location = new Point(10, 25),
                AutoSize = true,
                Parent = autoControlGroupBox
            };
            Button buttons = new Button
            {
                Size = floorButtonSize,
                //楼层数
                Text = "修改",
                Parent = autoControlGroupBox,
                Location = new Point(210, 25)
            };
            buttons.Click += changePersonNum;
            personNumeric = new NumericUpDown
            {
                Value = 0,
                Parent = autoControlGroupBox,
                Location = new Point(140, 25),
                Minimum = 0,
                Size = new Size(50, 50)
            };
            humanBeings = new List<HumanBeing>();

            //乘坐电梯
            waitingHumanImage = new PictureBox[buildingHeight];
            waitingHuman = new int[buildingHeight];
            GroupBox waitingBuilding = new GroupBox
            {
                Text = "等待者",
                Parent = this,
                Size = new Size(70, maxSize * (buildingHeight + 2)),
                Location = new Point(40, 100)
            };

            Size floorImageSize = new Size(maxSize, maxSize);
            for (int i = 0; i < buildingHeight; ++i)
            {
                waitingHumanImage[i] = new PictureBox
                {
                    Size = floorImageSize,
                    Parent = waitingBuilding,
                    Location =
                    new Point(10, maxSize * i + maxSize),
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                waitingHumanImage[i].Image = Image.FromFile("img//无人等待.png");
                waitingHuman[i] = 0;
            }

            //添加个人信息
            GroupBox infoGroupBox = new GroupBox()
            {
                Text = "@信息",
                Size = new Size(550, 60),
                Location = new Point(30, position.Y + floorButtonSize.Height + maxSize * (buildingHeight + 2) + 10),
                Parent = this
            };
            Label messageLabel = new Label
            {
                Font = new Font(FML, 13),
                Text = "1953729 吴浩泽  操作系统项目01——电梯调度",
                Parent = infoGroupBox,
                Location = new Point(10, 25),
                AutoSize = true
            };
            

            //说明框
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            MenuStrip menuStrip1 = new System.Windows.Forms.MenuStrip();
            ToolStripMenuItem toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem1});
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(83, 24);
            toolStripMenuItem1.Text = "说明";
            toolStripMenuItem1.Click += introClick;
            
            menuStrip1.Parent=this;
            this.MainMenuStrip = menuStrip1;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();

        }

        //内部按钮
        private void insideRequest(object sender, EventArgs e)
        {
            Button insideButton = (Button)sender;

            //获取编号和楼层信息
            string[] s = insideButton.Name.Split();
            int index = int.Parse(s[0]);
            int floor = int.Parse(s[1]);

            this.elevator[index].elevator.stopFloor[floor] = true;
            this.elevator[index].elevator.insideButton.Add(insideButton);

            insideButton.Enabled = false;
        }

        //外部按钮
        public void outsideRequest(object sender, EventArgs e)
        {
            Button requestButton = (Button)sender;

            //按下这个按钮后，按钮需要处于被激活状态
            requestButton.Enabled = false;

            bool toUp = requestButton.Name[0] == '+';
            int startFloor = int.Parse(requestButton.Name.Substring(1));
            if (toUp)
            {
                requestButton.BackgroundImage = Image.FromFile("img//向上-click.png");
            }
            else
            {
                requestButton.BackgroundImage = Image.FromFile("img//向下-click.png");
            }
            //提醒操作
            elevatorLog("L" + startFloor.ToString() + "请求"
                + (toUp ? "上楼" : "下楼"));

            //调度算法
            addRequest(startFloor, toUp, requestButton);
        }

        //强制开门
        private void openRequest(object sender, EventArgs e)
        {
            int index = int.Parse(((Button)sender).Name);
            elevatorLog((index + 1).ToString() + "号电梯请求开门");
            elevator[index].elevator.doorOpen();
        }

        //强制关门
        private void closeRequest(object sender, EventArgs e)
        {
            int index = int.Parse(((Button)sender).Name);
            elevatorLog((index + 1).ToString() + "号电梯请求关门");
            elevator[index].elevator.doorClose();
        }

        //警报按钮
        private void warningRequest(object sender, EventArgs e)
        {
            Button requestButton = (Button)sender;
            int index = int.Parse(requestButton.Name);

            if (!elevator[index].elevator.eleveatorWarning)
            {
                //提醒操作
                elevatorLog((index + 1).ToString() + "号电梯进入警报状态，中止全部任务");

                elevator[index].elevator.warningState();
                //暂时禁止全部按钮
                for (int i = 0; i < buildingHeight; ++i)
                {
                    elevator[index].floorButton[i].Enabled = false;
                    elevator[index].elevator.stopFloor[i] = false;
                }
                //清空待完成队列
                elevator[index].elevator.insideButton.Clear();

                //重新分配外部请求
                while (elevator[index].elevator.outsideButton.Count() != 0)
                {
                    Button reDistrubute = elevator[index].elevator.outsideButton[0];
                    bool toUp = reDistrubute.Name[0] == '+';
                    int startFloor = int.Parse(reDistrubute.Name.Substring(1));
                    elevator[index].elevator.outsideButton.RemoveAt(0);

                    //提醒操作
                    elevatorLog("重新为L" + startFloor.ToString() + "的"
                        + (toUp ? "上楼" : "下楼") + "请求进行调度");

                    //调度算法
                    addRequest(startFloor, toUp, reDistrubute);
                }

            }
            else
            {
                //提醒操作
                elevatorLog((index + 1).ToString() + "号电梯停止警报，恢复工作");
                //设置为门开状态，防止直接进入运行状态
                elevator[index].elevator.elevatorState = Elevator.ELEVATORSTATE.DOOROPENING;
                elevator[index].elevator.warningState();
                //恢复按钮
                for (int i = 0; i < buildingHeight; ++i)
                {
                    elevator[index].floorButton[i].Enabled = true;
                }
            }

            //调用调度算法
            scanAlgorithm();
        }

        //刷新
        private void restart(object sender, EventArgs e)
        {
            int newNum = int.Parse(elevatorNumeric.Value.ToString());
            int newHeight = int.Parse(heightNumeric.Value.ToString());
            if (newNum == elevatorNum && newHeight == buildingHeight)
            {
                MessageBox.Show("电梯数量和楼层高度未变化");
                return;
            }
            Form1 form = new Form1(newNum, newHeight);
            Hide();
            form.TopMost = true;
            form.Show();
        }

        public void elevatorLog(string information)
        {
            richTextBox.AppendText("\n◎  " + information);
        }

        private int abs(int x)
        {
            if (x > 0)
                return x;
            else
                return -x;
        }

        private void clearInformation(object sender, EventArgs e)
        {
            richTextBox.Text = "";
        }

        void scanAlgorithm()
        {

            for (int i = 0; i < requests.Count(); ++i)
            {
                //已分配电梯
                if (requests[i].elevatorIndex != -1)
                {
                    continue;
                }

                //寻找到距离最近运行方向相同或目前暂停的电梯
                int minIndex = -1;
                int minDist = buildingHeight + 1;
                for (int j = 0; j < elevatorNum; ++j)
                {
                    if (elevator[j].elevator.eleveatorWarning) //电梯被禁用
                    {
                        continue;
                    }
                    if (elevator[j].elevator.elevatorMovingState == Elevator.ELEVATORMOVINGSTATE.WAIT)
                    {
                        //等待状态的电梯，可以直接进行考虑
                        if (abs(elevator[j].elevator.floor - requests[i].startFloor) < minDist)
                        {
                            minDist = abs(elevator[j].elevator.floor - requests[i].startFloor);
                            minIndex = j;
                        }
                    }
                    else if (requests[i].up && elevator[j].elevator.elevatorMovingState == Elevator.ELEVATORMOVINGSTATE.UP
                        && elevator[j].elevator.floor <= requests[i].startFloor)
                    {
                        if (abs(elevator[j].elevator.floor - requests[i].startFloor) < minDist)
                        {
                            minDist = abs(elevator[j].elevator.floor - requests[i].startFloor);
                            minIndex = j;
                        }
                    }
                    else if (!requests[i].up && elevator[j].elevator.elevatorMovingState == Elevator.ELEVATORMOVINGSTATE.DOWN
                        && elevator[j].elevator.floor >= requests[i].startFloor)
                    {
                        if (abs(elevator[j].elevator.floor - requests[i].startFloor) < minDist)
                        {
                            minDist = abs(elevator[j].elevator.floor - requests[i].startFloor);
                            minIndex = j;
                        }
                    }

                }

                if (minIndex != -1)
                {
                    requests[i].elevatorIndex = minIndex;
                    elevator[minIndex].elevator.stopFloor[requests[i].startFloor] = true;
                    elevator[minIndex].elevator.outsideButton.Add(requests[i].requestButton);
                    if (elevator[minIndex].elevator.elevatorMovingState == Elevator.ELEVATORMOVINGSTATE.WAIT)
                    {
                        //让该电梯移动
                        if (requests[i].startFloor != elevator[minIndex].elevator.floor)
                        {
                            //将其加入到待执行行列中即可
                            elevator[minIndex].elevator.outsideButton.Add(requests[i].requestButton);
                        }
                        else
                        {
                            if (elevator[minIndex].elevator.elevatorState == Elevator.ELEVATORSTATE.DOORCLOSE)
                            {
                                elevator[minIndex].elevator.elevatorMovingState = Elevator.ELEVATORMOVINGSTATE.WAIT;
                                //开门
                                elevator[minIndex].elevator.elevatorState = Elevator.ELEVATORSTATE.DOOROPENING;
                            }
                            else
                            {
                                elevator[minIndex].elevator.elevatorMovingState = Elevator.ELEVATORMOVINGSTATE.WAIT;
                                //如果电梯已经处于开门状态
                                elevator[minIndex].elevator.elevatorState = Elevator.ELEVATORSTATE.DOOROPENING;
                            }
                        }

                    }
                    elevatorLog("为其调度了" + (minIndex + 1).ToString() + "号电梯");
                    requests.RemoveAt(i);
                    return;
                }

            }

            //确认已完成的指令
            for (int i = 0; i < requests.Count(); ++i)
            {
                if (requests[i].requestButton.Enabled)
                {
                    requests.RemoveAt(i);
                    return;
                }
            }
        }

        //增加命令
        void addRequest(int startFloor, bool toUp, Button requestButton)
        {
            //将其加入到request结构体中
            ElevatorRequest request = new ElevatorRequest(startFloor, toUp, requestButton);
            requests.Add(request);

        }

        //修改乘电梯人数
        void changePersonNum(object sender, EventArgs e)
        {
            int newNum = int.Parse(personNumeric.Value.ToString());
            if (newNum == humanBeings.Count)
            {
                MessageBox.Show("乘坐电梯人数未发生改变！");
                return;
            }
            if (newNum > humanBeings.Count)
            {
                while (newNum != humanBeings.Count)
                {
                    HumanBeing newPerson = new HumanBeing(buildingHeight, this);
                    humanBeings.Add(newPerson);
                }
                MessageBox.Show("修改成功，乘客即将到来");
            }
            else
            {
                while (newNum != humanBeings.Count)
                {
                    if (humanBeings[0].state == HumanBeing.STATE.TAKING)
                    {
                        elevator[humanBeings[0].selectedElevator].elevator.personNum--;
                        elevator[humanBeings[0].selectedElevator].elevator.updateInfo();

                    }
                    else if (humanBeings[0].state == HumanBeing.STATE.WAITING)
                    {
                        waitingHuman[humanBeings[0].maxHeight - humanBeings[0].startFloor]--;
                        if (waitingHuman[humanBeings[0].maxHeight - humanBeings[0].startFloor] == 0)
                        {
                            waitingHumanImage[humanBeings[0].maxHeight - humanBeings[0].startFloor].Image = Image.FromFile("img//无人等待.png");
                        }
                    }
                    humanBeings[0].personTimer.Stop();
                    humanBeings.RemoveAt(0);
                }
                MessageBox.Show("修改成功，已移除乘客");
            }

        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            scanAlgorithm();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void introClick(object sender, EventArgs e)
        {
            MessageBox.Show("√	多楼层多电梯的调度\n√	电梯信息的显示\n√	乘客自动乘梯机制：在右下角设置\n√	相关参数的更改：在右上角更改相关参数\n√	警报功能\n学号：1953729\n姓名：吴浩泽", "说明", MessageBoxButtons.OK);
        }
    }
}
