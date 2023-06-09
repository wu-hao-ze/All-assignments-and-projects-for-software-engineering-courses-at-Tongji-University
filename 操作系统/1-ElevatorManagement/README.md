# 电梯调度-说明文档

## 项目说明

### 项目简介

通过控制电梯调度，本项目模拟了操作系统调度过程，此外还实现了乘客自动乘坐电梯的机制。

### 项目功能

本项目实现了以下的功能：

- **多楼层多电梯的调度**

  项目实现了多个电梯在多个楼层之间的调度，并且可以同时通过外部按钮和内部按钮（包括楼层按钮、开关门、警报）双重控制电梯的状态

- **电梯信息的显示**

  电梯的楼层以及当前运行状态会显示在电梯的上部。此外，鼠标移动到电梯上还能够看到当前承载人数和所在楼层

- **乘客自动乘梯机制**

  在右下角设置乘客人数，便会每隔一段时间刷新出一定数量的乘客需要乘梯，他们会自动按下自己所需要前往的楼层

- **相关参数的更改**

  在右上角更改相关参数，可以查看不同楼层和电梯数的调度

- **警报功能**

  当某一电梯处于警报状态，将开门并被暂停使用，同时该电梯的乘客离开

### 项目环境

- **开发环境**

  C#.NET Windows窗体

  Windows版本10.0.18363.1500

- **运行环境**

  请在Windows7及以上版本运行，于Windows10运行最佳

- **运行方法**

  打开电梯调度.exe即可运行本项目，使用说明请参照菜单栏

## 功能实现

### 实体介绍

本项目中共分为三个实体：电梯、乘客和窗体。

针对于**电梯**（Elevator），它主要包含开关门方法、向上和向下移动方法、警报方法，它有下述几个状态：

| 向上移动                                                 | 向下移动                                                 | 警报                                                     | 开关门                                                   |
| -------------------------------------------------------- | -------------------------------------------------------- | -------------------------------------------------------- | -------------------------------------------------------- |
| ![gTreyQ.png](https://z3.ax1x.com/2021/05/20/gTreyQ.png) | ![gTrKwn.png](https://z3.ax1x.com/2021/05/20/gTrKwn.png) | ![gTrBY6.png](https://z3.ax1x.com/2021/05/20/gTrBY6.png) | ![gTr2md.png](https://z3.ax1x.com/2021/05/20/gTr2md.png) |

为了方便的标识每个电梯的状态，电梯类内定义了几个不同的状态变量，如下所示：

|       变量名        |        类型         |                             功能                             |
| :-----------------: | :-----------------: | :----------------------------------------------------------: |
|    elevatorState    |    ELEVATORSTATE    | 标识电梯当前的运行状态，包括移动、门已开、开门中、门已关、关门中 |
| elevatorMovingState | ELEVATORMOVINGSTATE |            标识电梯移动状态，包括向上、向下和暂停            |
|      doorState      |      DOORSTATE      |             标识电梯门的状态，用于电梯开关门动画             |

此外，电梯类内部还有以下几个Timer计时器控件对电梯状态进行控制：

|      变量名       | 时间间隔 |                    功能                    |
| :---------------: | :------: | :----------------------------------------: |
|    globalTimer    |    1     |  全局计时器，根据电梯当前状态执行相应操作  |
|    movingTimer    |    15    | 移动计时器，电梯移动时更改电梯所在位置坐标 |
|     doorTimer     |   500    |  开关门计时器，在电梯开关门时更改门的图形  |
| movingNoticeTimer |   250    |    标识计时器，用于显示电梯当前状态动画    |

电梯被分配外部任务时，该任务会被加入到List< Button >  outsideButton当中去；电梯内部按钮被按下时，则会被加入到List< Button >  insideButton当中。电梯在移动过程中，每到达一个楼层便会根据内部和外部任务检查是否需要停靠。

针对于**乘客**（HumanBeing）类，它出现在动态乘坐电梯的过程当中，它会每隔一段时间（随机2000~4000）需要从起始楼层到某一终止楼层，并且会自动按下电梯按键。包含以下几个状态：

|                         尚未到达                         |                           等待                           |                         乘坐电梯                         |
| :------------------------------------------------------: | :------------------------------------------------------: | :------------------------------------------------------: |
| ![gTs9cF.png](https://z3.ax1x.com/2021/05/20/gTs9cF.png) | ![gTsinJ.png](https://z3.ax1x.com/2021/05/20/gTsinJ.png) | ![gTsCX4.png](https://z3.ax1x.com/2021/05/20/gTsCX4.png) |

而为了实现乘客的上述功能，其内部定义了以下关键变量：

|      变量名      | 类型  |                   功能                   |
| :--------------: | :---: | :--------------------------------------: |
|      state       | STATE | 标识乘客当前的状态，即上述提到的三个状态 |
|    startFloor    |  int  |         乘客出发的楼层，随机生成         |
|     endFloor     |  int  |         乘客的目标楼层，随机生成         |
|   personTimer    | Timer | 状态计时器，用于控制乘客在几个状态间转换 |
| selectedElevator |  int  |       按下目标按钮后，被分配的电梯       |

而对于**窗体**（Form）类，除了定义上述提到了两个实体电梯和乘客外，它还定义了结构体ElevatorRequest，用于存储当前所要调度的任务，内部变量如下：

|    变量名     |  类型  |                    功能                    |
| :-----------: | :----: | :----------------------------------------: |
|  startFloor   |  int   |               任务的起始楼层               |
|      up       |  bool  |            任务需要上楼还是下楼            |
| elevatorIndex |  int   | 该任务被分配执行的电梯编号，-1表示尚未分配 |
| requestButton | Button | 该任务对应的外部按钮，用于传递给分配的电梯 |

此外，窗体类内部还定义了以下关键变量：

|     变量名     |         类型          |            功能            |
| :------------: | :-------------------: | :------------------------: |
|    requests    | List<ElevatorRequest> | 存储当前需要进行调度的任务 |
| algorithmTimer |         Timer         |  调度算法计时器，分配任务  |

### 调度算法

本项目中的核心调度算法采用了LOOK算法的思想：

> LOOK算法是扫描算法（SCAN）的一种改进，扫描算法（SCAN） 是一种按照楼层顺序依次服务请求，它让电梯在最底层和最顶层之间连续往返运行，在运行过程中响应处在于电梯运行方向相同的各楼层上的请求。但当 LOOK 算法发现电梯所移动的方向上不再有请求时立即改变运行方向，而扫描算法则需要移动到最底层或者最顶层时才改变运行方向。

在本项目中，LOOK算法会检查当前所有电梯Elevator的状态elevatorState，从中选取一个与需要进行调度任务的运行方向相同并且还未超过运行方向或目前处于暂停状态中的电梯（该电梯目前不处于警报状态）里，距离该任务最近的一个电梯，具体代码详见函数**scanAlgorithm()**。

### 开/关门功能

若在电梯上升或下降时或处于警报状态执行开门按钮，按钮无效；若电梯开门停靠且不处于警报状态时，开门按钮将打开电梯的门，在停靠数秒后自动关闭；关闭按钮与此对应，只有当电梯的门已经处于打开或者正在被打开的状态时，关门按钮才能够发挥作用。

### 警报功能

无论电梯处于什么状态，当电梯内部按下警报按钮后，电梯立刻停在当前所处的楼层并自动打开门，所有正在乘坐该电梯的乘客离开电梯。同时，之前已被分配的任务将会被收回，**重新进行调度**，而全部内部任务将会被取消。只有当再次点击按钮取消警报状态后，电梯才能够恢复正常运行状态。
### 参数修改

本项目中，楼层数和电梯数被定义为变量。与此同时，所有控件都是动态生成的，因此可以通过在右上角修改相关参数，查看不同楼层数和电梯数的电梯调度系统。

### 乘客乘梯功能

为了模拟现实世界存在大量乘客乘梯的情况，如2.1中所述项目中定义了乘客类HumanBeing。在界面右下角，可以设置当前自动乘梯人数，系统将会生成相应的乘客。为了便于观察，将鼠标移动到电梯上部还能查看到相应电梯的信息。

每隔一段时间（随机2000~4000），乘客会随机出现在某一楼层，并且需要到达某一重点楼层。在乘坐电梯的过程中，乘客会自动按下相应楼层的按键；而当电梯进入警报状态后，乘客将会自动离开电梯。



## 项目总结

### 项目亮点

- 实现了多楼层多电梯之间的调度
- 动画完整生动，能够展示开关门、警报、楼梯移动
- 类封装较好，代码复用性高
- 具有警报键，警报键按下后，本电梯长时间开门停靠于当前所处楼层
- 具有日志输出，可以查看当前调度动态
- 动态生成，可以调整楼层高度和电梯数量
- 模拟乘客功能，可以体验不同人流量下的电梯运作状态
- 当警报响起后，模拟乘客将会立马离开，模拟了现实世界的乘客疏散

### 项目改进方向

LOOK算法在本项目中整体发挥比较好，但是未来可以考虑根据实际情况（例如乘客数量过多等）进行不同调度算法的应用，从而进一步提高电梯调度的效率。

此外，本项目由于框架的使用，仅限于在Windows平台运行。在未来项目开发过程中，可以考虑采取其他开发工具，使得项目可移植性更好。