# listwidget.pyw
from typing import Optional
from PyQt5.QtWidgets import QListWidget, QWidget, QAbstractItemView, QListWidgetItem
from PyQt5.QtGui import QDragEnterEvent, QDropEvent, QDragMoveEvent, QKeyEvent
from PyQt5.QtCore import Qt, QModelIndex
 

class MyListWidget(QListWidget):
    """支持拖拽的QListWidget"""
    def __init__(self, curNode,parents,parent: Optional[QWidget]=None) -> None:
        super().__init__(parent)
        # 拖拽设置
        self.setAcceptDrops(True)
        self.setDragEnabled(True)
        self.setDragDropMode(QAbstractItemView.DragDrop)            # 设置拖放
        self.setSelectionMode(QAbstractItemView.ExtendedSelection)  # 设置选择多个
        self.setDefaultDropAction(Qt.CopyAction)
        # 双击可编辑
        self.edited_item = self.currentItem()
        self.close_flag = True
        #self.doubleClicked.connect(self.item_double_clicked)
        self.currentItemChanged.connect(self.close_edit)
        #当前目录
        self.curNode=curNode
        #父亲
        self.parents=parents
        #正在被编辑状态
        self.isEdit=False
    
    def keyPressEvent(self, e: QKeyEvent) -> None:
        """回车事件，关闭edit"""
        super().keyPressEvent(e)
        if e.key() == Qt.Key_Return:
            if self.close_flag:
                self.close_edit()
            self.close_flag = True

    

    def edit_new_item(self) -> None:
        """edit一个新的item"""
        self.close_flag = False
        self.close_edit()
        count = self.count()
        self.addItem('')
        item = self.item(count)
        self.edited_item = item
        self.openPersistentEditor(item)
        self.editItem(item)

    def item_double_clicked(self, modelindex: QModelIndex) -> None:
        #双击需要被ban掉
        return
        
    def editLast(self,index=-1)->None:
        self.close_edit()
        item = self.item(self.count()-1)
        self.setCurrentItem(item)
        self.edited_item = item
        self.openPersistentEditor(item)
        self.editItem(item)
        self.isEdit=True
        self.index=index
        
    def editSelected(self,index)->None:
        self.close_edit()
        item = self.selectedItems()[-1]
        self.setCurrentItem(item)
        self.edited_item = item
        self.openPersistentEditor(item)
        self.editItem(item)
        self.isEdit=True
        self.index=index

    def close_edit(self, *_) -> None:
        """关闭edit"""
        if self.edited_item:
            self.isEdit=False
            self.closePersistentEditor(self.edited_item)
            #检验是否重名
            print(self.curNode.children)
            while True:
                sameName=False
                for i in range(len(self.curNode.children)-1):
                    if self.edited_item.text()==self.curNode.children[i].name and self.index!=i:
                        self.edited_item.setText(self.edited_item.text()+"(2)")
                        sameName=True
                        print('same name')
                        break
                if not sameName:
                    break
            
            #计算item在其父结点的下标
            
            self.curNode.children[self.index].name=self.edited_item.text()
            #更新父目录
            self.parents.updateTree()
            
            self.edited_item=None


    def dragEnterEvent(self, e: QDragEnterEvent) -> None:
        """（从外部或内部控件）拖拽进入后触发的事件"""
        # print(e.mimeData().text())
        if e.mimeData().hasText():
            if e.mimeData().text().startswith('file:///'):
                e.accept()
        else:
            e.ignore()

    def dragMoveEvent(self, e: QDragMoveEvent) -> None:
        """拖拽移动过程中触发的事件"""
        e.accept()

    def dropEvent(self, e: QDropEvent) -> None:
        """拖拽结束以后触发的事件"""
        paths = e.mimeData().text().split('\n')
        for path in paths:
            path = path.strip()
            if len(path) > 8:
                self.addItem(path.strip()[8:])
        e.accept()

