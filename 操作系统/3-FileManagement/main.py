import sys
from PyQt5.QtWidgets import *
from PyQt5.QtGui import QIcon,QStandardItem,QStandardItemModel
from PyQt5.QtCore import QSize
from PyQt5.Qt import *
from File import *
import time
import os
import pickle
from MyWidget import MyListWidget
from fileEdit import editForm,attributeForm

#主窗体
class mainForm(QMainWindow):
    def __init__(self):
        super().__init__()

        """
        读取文件
        """
        self.readFile()

        #根目录
        self.curNode=self.catalog[0]
        self.rootNode=self.curNode
        self.baseUrl=['root']

        """
        窗体基本信息
        """
        self.resize(1200,800)
        self.setWindowTitle('操作系统项目3之文件管理')
        self.setWindowIcon(QIcon('img/folder.ico'))

        #窗口居中
        qr=self.frameGeometry()
        centerPlace=QDesktopWidget().availableGeometry().center()
        qr.moveCenter(centerPlace)
        self.move(qr.topLeft())

        #窗口布局
        grid=QGridLayout()
        grid.setSpacing(10)
        self.widGet=QWidget()
        self.widGet.setLayout(grid)
        self.setCentralWidget(self.widGet)
        
        #退出事件
        exitAction = QAction(QIcon('file.png'), '直接退出', self)        
        exitAction.setShortcut('Ctrl+Q')
        exitAction.triggered.connect(qApp.quit)

        #菜单栏
        menubar=self.menuBar()
        #添加菜单
        fileMenu=menubar.addMenu('文件')
        #添加事件
        fileMenu.addAction(exitAction)
        
        menubar.addAction('格式化', self.format)
        
        menubar.addAction('说明', self.introduction)

        """
        添加工具栏
        """

        #返回键
        self.backAction=QAction(QIcon('img/back.png'), '&返回',self)
        exitAction.setShortcut('Ctrl+E')
        self.backAction.triggered.connect(self.backEvent)
        self.toolBar=self.addToolBar('工具栏')
        self.toolBar.addAction(self.backAction)
        self.backAction.setEnabled(False)

        #前进键
        self.forwardAction=QAction(QIcon('img/forward.png'), '&前进',self)
        self.forwardAction.triggered.connect(self.forwardEvent)
        self.toolBar.addAction(self.forwardAction)
        self.forwardAction.setEnabled(False)

        self.toolBar.addSeparator()

        #当前所在路径
        self.curLocation=QLineEdit()
        self.curLocation.setText('> root')
        self.curLocation.setReadOnly(True)

        #图标
        self.curLocation.addAction(QIcon('img/folder.png'), QLineEdit.LeadingPosition)

        self.curLocation.setMinimumHeight(40)
        ptrLayout=QFormLayout()
        ptrLayout.addRow(self.curLocation)

        ptrWidget=QWidget()

        ptrWidget.setLayout(ptrLayout) 
        ptrWidget.adjustSize()
        #设置自动补全
        self.toolBar.addWidget(ptrWidget)

        self.toolBar.setMovable(False)

        """
        左侧地址栏
        """

        #左侧的地址栏
        self.tree=QTreeWidget()
        #设置列数
        self.tree.setColumnCount(1)
        #设置标题
        self.tree.setHeaderLabels(['地址'])
        #建树
        self.buildTree()
        #设置选中状态
        self.tree.setCurrentItem(self.rootItem)
        #设置当前路径
        self.treeItem=[self.rootItem]
        #绑定单击事件
        self.tree.itemClicked['QTreeWidgetItem*','int'].connect(self.clickTreeItem)

        grid.addWidget(self.tree,1,0)


        """
        文件基本信息
        """
        self.listView=MyListWidget(self.curNode,parents=self)
        self.listView.setMinimumWidth(800)
        self.listView.setViewMode(QListView.IconMode)
        self.listView.setIconSize(QSize(72,72))
        self.listView.setGridSize(QSize(100,100))
        self.listView.setResizeMode(QListView.Adjust)
        self.listView.setMovement(QListView.Static)
        self.listView.setEditTriggers(QAbstractItemView.AllEditTriggers)
        self.listView.doubleClicked.connect(self.openFile)

        #加载当前路径文件
        self.loadCurFile()
        grid.addWidget(self.listView, 1, 1)

       

        """
        右击菜单
        """

        self.listView.setContextMenuPolicy(Qt.CustomContextMenu)
        self.listView.customContextMenuRequested.connect(self.show_menu)

        """
        美化
        """
        self.updatePrint()
        self.lastLoc=-1

        #删除文件快捷键
        QShortcut(QKeySequence(self.tr("Delete")), self, self.deleteFile)
    
    def introduction(self):
        QMessageBox.about(self, '说明', 
        '本项目为软件学院2019级操作系统课程项目3的文件管理\n\n'+
        '项目通过模拟FAT表、多级目录结构等文件管理中使用的策略，完成了一个基本的文件管理系统\n'+
        '-----------------------------------------\n'+
        '打开文件：右击选中或者双击打开文件\n'+
        '删除文件：右击选中或者通过Delete快捷键删除文件\n'+
        '左侧地址栏：查看多级目录结构，并且能够在此点击快速跳转\n'+
        '属性：选中文件右击可以查看该文件属性，不选中可以查看当前所在文件夹的属性\n'+
        '新建文件：右击选中即可新建文件或文件夹\n'+
        '重命名:右击选中需要重命名的文件或文件夹即可重命名\n'+
        '格式化：清空所有内容\n'+
        '导航栏：查看当前所在路径\n'+
        '后退/前进：返回上一级目录/前往上一步去过的目录\n'+
        '-----------------------------------------\n'+
        '学号：1851055\n'+
        '姓名：汪明杰')

    def clickTreeItem(self,item,column):

        ways=[item]
        #查看所在层数
        level=0
        temp=item
        
        while temp.parent()!=None:
            temp=temp.parent()
            ways.append(temp)
            level+=1
 
        ways.reverse()
        #回退到根节点
        while self.backEvent():
            pass
        self.baseUrl=self.baseUrl[:1]
        self.treeItem=self.treeItem[:1]

        #一步一步前进
        for i in ways:
            if i==self.rootItem:
                continue
            #前往该路径
            #从curNode中查询item
            newNode=None
            for j in self.curNode.children:
                if j.name==i.text(0):
                    newNode=j
                    break
            #前往路径j
            if newNode.isFile:
                #文件的话，break即可
                break
            else:
                self.curNode=newNode
                self.updateLoc()
                self.baseUrl.append(newNode.name)

                #更新路径
                for j in range(self.treeItem[-1].childCount()):
                    if self.treeItem[-1].child(j).text(0)==newNode.name:
                        selectedItem=self.treeItem[-1].child(j)
                self.treeItem.append(selectedItem)
                self.tree.setCurrentItem(selectedItem)
        
        #更新下标
        self.updatePrint()
        
        if self.curNode!=self.rootNode:
            self.backAction.setEnabled(True)
        
        self.forwardAction.setEnabled(False)
        self.lastLoc=-1

    def updateLoc(self):
        self.loadCurFile()
        self.listView.curNode=self.curNode

    #打开文件
    def openFile(self,modelindex: QModelIndex)->None:
        #获取点击item
        self.listView.close_edit()

        try:
            item = self.listView.item(modelindex.row())
        except:
            #报错，则说明是右键打开方式
            if len(self.listView.selectedItems())==0:
                return
            item=self.listView.selectedItems()[-1]

        #如果可以前进
        if self.lastLoc!=-1 and self.nextStep:
            item=self.listView.item(self.lastLoc)
            self.lastLoc=-1
            self.forwardAction.setEnabled(False)
        self.nextStep=False

        newNode=None
        for i in self.curNode.children:
            if i.name==item.text():
                newNode=i
                break

        if newNode.isFile:
            data=newNode.data.read(self.fat,self.disk)
            self.child=editForm(newNode.name, data)
            self.child._signal.connect(self.getData)
            self.child.show()
            self.writeFile=newNode
        else:
            #进下一级目录前，如果处于编辑状态，一定要取消编辑
            self.listView.close_edit()

            self.curNode=newNode
            self.updateLoc()
            self.baseUrl.append(newNode.name)

            #更新路径
            for i in range(self.treeItem[-1].childCount()):
                if self.treeItem[-1].child(i).text(0)==newNode.name:
                    selectedItem=self.treeItem[-1].child(i)
            self.treeItem.append(selectedItem)
            self.tree.setCurrentItem(selectedItem)
            self.backAction.setEnabled(True)

            self.updatePrint()

    def updatePrint(self):
        self.statusBar().showMessage(str(len(self.curNode.children))+'个项目'+
        '                                                                   '+
        '                                                                   '+
        '                                                                   '+
        '学号:1851055            姓名:汪明杰')
        s='> root'
        for i,item in enumerate(self.baseUrl):
            if i==0:
                continue
            s+=" > "+item
                
        self.curLocation.setText(s)

    def rename(self):
        if len(self.listView.selectedItems())==0:
            return
        #获取最后一个被选中的
        self.listView.editSelected(self.listView.selectedIndexes()[-1].row())
        self.updateTree()

    def deleteFile(self):
        """
        删除文件
        """
        if len(self.listView.selectedItems())==0:
            return

        item=self.listView.selectedItems()[-1]
        index=self.listView.selectedIndexes()[-1].row()

        #提示框
        reply=QMessageBox()
        reply.setWindowTitle('提醒')
        #不同的题型
        if self.curNode.children[index].isFile:
            reply.setText('确定要删除文件'+item.text()+'吗？')
        else:
            reply.setText('确定要删除文件夹'+item.text()+'及其内部所有内容吗？')
        reply.setStandardButtons(QMessageBox.Yes | QMessageBox.No)
        buttonY = reply.button(QMessageBox.Yes)
        buttonY.setText('确定')
        buttonN = reply.button(QMessageBox.No)
        buttonN.setText('取消')

        reply.exec_()

        if reply.clickedButton()==buttonN:
            return
        
        #删除文件
        self.listView.takeItem(index)
        del item
        #删除fat表中的内容
        self.deleteFileRecursive(self.curNode.children[index])
        self.curNode.children.remove(self.curNode.children[index])
        #更新catalog表
        self.catalog=self.updateCatalog(self.rootNode)

        #更新
        self.updateTree()

    def deleteFileRecursive(self,node):
        if node.isFile:
            node.data.delete(self.fat,self.disk)
        else:
            for i in node.children:
                self.deleteFileRecursive(i)
   

    def updateCatalog(self,node):
        if node.isFile:
            return [node]
        else:
            x=[node]
            for i in node.children:
                x+=self.updateCatalog(i)
            return x

    def createFolder(self):

        self.item_1=QListWidgetItem(QIcon("img/folder.png"), "新建文件夹")
        self.listView.addItem(self.item_1)
        self.listView.editLast()

        #添加到目录表中
        newNode=CatalogNode(self.item_1.text(),False,self.fat,self.disk,time.localtime(time.time()),self.curNode)
        self.curNode.children.append(newNode)
        self.catalog.append(newNode)

        #更新树
        self.updateTree()

    def createFile(self):
        self.item_1=QListWidgetItem(QIcon("img/file.png"), "新建文件")
        self.listView.addItem(self.item_1)
        self.listView.editLast()

        #添加到目录表中
        newNode=CatalogNode(self.item_1.text(),True,self.fat,self.disk,time.localtime(time.time()),self.curNode)
        self.curNode.children.append(newNode)
        self.catalog.append(newNode)

        #更新树
        self.updateTree()

    def viewAttribute(self):
        #查看当前路径属性
        if len(self.listView.selectedItems())==0:
            self.child=attributeForm(self.curNode.name, False,self.curNode.createTime,self.curNode.updateTime,len(self.curNode.children))

            self.child.show()
            return
        else:
            #获取选中的最后一个
            node=self.curNode.children[self.listView.selectedIndexes()[-1].row()]
            if node.isFile:
                self.child=attributeForm(node.name, node.isFile,node.createTime,node.updateTime,0)
            else:
                self.child=attributeForm(node.name, node.isFile,node.createTime,node.updateTime,len(node.children))
            self.child.show()
            return

    
    def show_menu(self,point):
        menu=QMenu(self.listView)
        
        #选中了一些元素
        if len(self.listView.selectedItems())!=0:
            """
            打开文件
            """

            openFileAction=QAction(QIcon(),'打开')
            openFileAction.triggered.connect(self.openFile)
            menu.addAction(openFileAction)

            deleteAction=QAction(QIcon(),'删除')
            deleteAction.triggered.connect(self.deleteFile)
            menu.addAction(deleteAction)


            renameAction=QAction(QIcon(),'重命名')
            renameAction.triggered.connect(self.rename)
            menu.addAction(renameAction)

            viewAttributeAction=QAction(QIcon('img/attribute.png'),'属性')
            viewAttributeAction.triggered.connect(self.viewAttribute)
            menu.addAction(viewAttributeAction)

            dest_point=self.listView.mapToGlobal(point)
            menu.exec_(dest_point)

        else:
            """
            查看
            """
            viewMenu=QMenu(menu)
            viewMenu.setTitle('查看')
            #大图标
            bigIconAction=QAction(QIcon(),'大图标')
            def bigIcon():
                self.listView.setIconSize(QSize(172,172))
                self.listView.setGridSize(QSize(200,200))
            bigIconAction.triggered.connect(bigIcon)
            viewMenu.addAction(bigIconAction)

            #大图标
            middleIconAction=QAction(QIcon(),'中等图标')
            def middleIcon():
                self.listView.setIconSize(QSize(72,72))
                self.listView.setGridSize(QSize(100,100))
            middleIconAction.triggered.connect(middleIcon)
            viewMenu.addAction(middleIconAction)

            #小图标
            smallIconAction=QAction(QIcon(),'小图标')
            def smallIcon():
                self.listView.setIconSize(QSize(56,56))
                self.listView.setGridSize(QSize(84,84))
            smallIconAction.triggered.connect(smallIcon)
            viewMenu.addAction(smallIconAction)


            menu.addMenu(viewMenu)
            """
            新建
            """

            createMenu=QMenu(menu)
            createMenu.setTitle('新建')

            #新建文件夹
            createFolderAction=QAction(QIcon('img/folder.png'),'文件夹')
            createFolderAction.triggered.connect(self.createFolder)
            createMenu.addAction(createFolderAction)

            #新建文件
            createFileAction=QAction(QIcon('img/file.png'),'文件')
            createFileAction.triggered.connect(self.createFile)
            createMenu.addAction(createFileAction)

            createMenu.setIcon(QIcon('img/create.png'))
            menu.addMenu(createMenu)


            """
            属性
            """
            viewAttributeAction=QAction(QIcon('img/attribute.png'),'属性')
            viewAttributeAction.triggered.connect(self.viewAttribute)
            menu.addAction(viewAttributeAction)

            self.nextStep=False
            

            dest_point=self.listView.mapToGlobal(point)
            menu.exec_(dest_point)

    def updateTree(self):
        node=self.rootNode
        item=self.rootItem

        if item.childCount()<len(node.children):
            #增加一个新item即可
            child=QTreeWidgetItem(item)
        elif item.childCount()>len(node.children):
            #一个一个找，删除掉对应元素
            for i in range(item.childCount()):
                if i==item.childCount()-1:
                    item.removeChild(item.child(i))
                    break
                if item.child(i).text(0)!=node.children[i].name:
                    item.removeChild(item.child(i))
                    break

        for i in range(len(node.children)):
            self.updateTreeRecursive(node.children[i], item.child(i))

        self.updateTreeRecursive(node, item)

    def updateTreeRecursive(self,node:CatalogNode,item:QTreeWidgetItem):
        item.setText(0, node.name)
        if node.isFile:
            item.setIcon(0, QIcon('img/file.png'))
        else:
            #根据是否有子树设置图标
            if len(node.children)==0:
                item.setIcon(0, QIcon('img/folder.png'))
            else:
                item.setIcon(0, QIcon('img/folderWithFile.png'))
            if item.childCount()<len(node.children):
                #增加一个新item即可
                child=QTreeWidgetItem(item)
            elif item.childCount()>len(node.children):
                #一个一个找，删除掉对应元素
                for i in range(item.childCount()):
                    if i==item.childCount()-1:
                        item.removeChild(item.child(i))
                        break
                    if item.child(i).text(0)!=node.children[i].name:
                        item.removeChild(item.child(i))
                        break
            for i in range(len(node.children)):
                self.updateTreeRecursive(node.children[i], item.child(i))


    def buildTree(self):
        self.tree.clear()
        self.rootItem=self.buildTreeRecursive(self.catalog[0],self.tree)
        #加载根节点的所有子控件
        self.tree.addTopLevelItem(self.rootItem)
        self.tree.expandAll()

    def getData(self, parameter):
        """
        向文件中写入新数据
        """
        self.writeFile.data.update(parameter,self.fat,self.disk)
        self.writeFile.updateTime=time.localtime(time.time())
        
    def buildTreeRecursive(self,node:CatalogNode,parent:QTreeWidgetItem):
        """
        目录树的建立
        """
        child=QTreeWidgetItem(parent)
        child.setText(0,node.name)

        if node.isFile:
            child.setIcon(0,QIcon('img/file.png'))
        else:
            if len(node.children)==0:
                child.setIcon(0, QIcon('img/folder.png'))
            else:
                child.setIcon(0, QIcon('img/folderWithFile.png'))
            for i in node.children:
                self.buildTreeRecursive(i,child)
        
        return child


    def loadCurFile(self):
        """
        加载当前路径的文件
        """
        self.listView.clear()

        for i in self.curNode.children:
            if i.isFile:
                self.item_1=QListWidgetItem(QIcon("img/file.png"), i.name)
                self.listView.addItem(self.item_1)
            else:
                if len(i.children)==0:
                    self.item_1=QListWidgetItem(QIcon("img/folder.png"), i.name)
                else:
                    self.item_1=QListWidgetItem(QIcon("img/folderWithFile.png"), i.name)
                self.listView.addItem(self.item_1)

    def format(self):
        """
        对象的格式化
        """ 
        #结束编辑
        self.listView.close_edit()

        #提示框
        reply=QMessageBox()
        reply.setWindowTitle('提醒')
        reply.setText('确定要格式化磁盘吗？(此操作不可逆！)')
        reply.setStandardButtons(QMessageBox.Yes | QMessageBox.No)
        buttonY = reply.button(QMessageBox.Yes)
        buttonY.setText('确定')
        buttonN = reply.button(QMessageBox.No)
        buttonN.setText('取消')
        reply.exec_()
        reply.show()

        if reply.clickedButton()==buttonN:
            return
        
        """
        格式化文件
        """
        self.fat=FAT()
        self.fat.fat=[-2]*blockNum
        #存储fat表
        with open('fat','wb') as f:
            f.write(pickle.dumps(self.fat))

        self.disk=[]
        for i in range(blockNum):
            self.disk.append(Block(i))
        #存储disk表
        with open('disk','wb') as f:
            f.write(pickle.dumps(self.disk))
        
        self.catalog=[]
        self.catalog.append(CatalogNode("root", False, self.fat, self.disk, time.localtime(time.time())))
        #存储
        with open('catalog','wb') as f:
            f.write(pickle.dumps(self.catalog))

        self.hide()
        self.winform=mainForm()
        self.winform.show()
        
    
    def saveFile(self):
        """
        将内存中的文件存到本地
        """
        #存储fat表
        with open('fat','wb') as f:
            f.write(pickle.dumps(self.fat))
        #存储disk表
        with open('disk','wb') as f:
            f.write(pickle.dumps(self.disk))
        #存储
        with open('catalog','wb') as f:
            f.write(pickle.dumps(self.catalog))

    def readFile(self):
        #读取fat表
        if not os.path.exists('fat'):
            self.fat=FAT()
            self.fat.fat=[-2]*blockNum
            #存储fat表
            with open('fat','wb') as f:
                f.write(pickle.dumps(self.fat))
        else:
            with open('fat','rb') as f:
                self.fat=pickle.load(f)

        #读取disk表
        if not os.path.exists('disk'):
            self.disk=[]
            for i in range(blockNum):
                self.disk.append(Block(i))
            #存储disk表
            with open('disk','wb') as f:
                f.write(pickle.dumps(self.disk))
        else:
            with open('disk','rb') as f:
                self.disk=pickle.load(f)

        #读取catalog表
        if not os.path.exists('catalog'):
            self.catalog=[]
            self.catalog.append(CatalogNode("root", False, self.fat, self.disk, time.localtime(time.time())))
            #存储
            with open('catalog','wb') as f:
                f.write(pickle.dumps(self.catalog))
        else:
            with open('catalog','rb') as f:
                self.catalog=pickle.load(f)

    def initial(self):
        # fat表
        self.fat=FAT()
        self.fat.fat=[-2]*blockNum
        #存储fat表
        with open('fat','ab') as f:
            f.write(pickle.dumps(self.fat))
        
        #disk表
        self.disk=[]
        for i in range(blockNum):
            self.disk.append(Block(i))
        #存储disk表
        with open('disk','ab') as f:
            f.write(pickle.dumps(self.disk))
        
        #catalogNode
        self.catalog=[]
        self.catalog.append(CatalogNode("root", False, self.fat, self.disk, time.localtime(time.time())))
        #存储
        with open('catalog','ab') as f:
            f.write(pickle.dumps(self.catalog))

    
    def backEvent(self):
        """
        返回上一级
        """
        self.listView.close_edit()

        if self.rootNode==self.curNode:
            #根节点无法返回
            return False


        #记录上次所在位置
        for i in range(len(self.curNode.parent.children)):
            if self.curNode.parent.children[i].name==self.curNode.name:
                self.lastLoc=i
                self.forwardAction.setEnabled(True)
                break

        self.curNode=self.curNode.parent
        self.updateLoc()
        self.baseUrl.pop()
        self.treeItem.pop()
        self.tree.setCurrentItem(self.treeItem[-1])
        self.updateTree()
        self.updatePrint()

        if self.curNode==self.rootNode:
            self.backAction.setEnabled(False)
        
        

        return True

    def forwardEvent(self):
        self.nextStep=True
        self.openFile(QModelIndex())

    def closeEvent(self,event):
        #结束编辑
        self.listView.close_edit()

        reply=QMessageBox()
        reply.setWindowTitle('提醒')
        reply.setText('您是否需要将本次操作写入磁盘？')
        reply.setStandardButtons(QMessageBox.Yes | QMessageBox.No | QMessageBox.Ignore)
        buttonY = reply.button(QMessageBox.Yes)
        buttonY.setText('写入')
        buttonN = reply.button(QMessageBox.No)
        buttonN.setText('取消')
        buttonI=reply.button(QMessageBox.Ignore)
        buttonI.setText('不写入')

        reply.exec_()

        if reply.clickedButton()==buttonI:
            event.accept()
        elif reply.clickedButton()==buttonY:
            self.saveFile()
            event.accept()
        else:
            event.ignore()


if __name__=='__main__':
    app=QApplication(sys.argv)

    mainform=mainForm()

    mainform.show()

    sys.exit(app.exec_())

