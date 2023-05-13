"""
文件管理所需的相关数据
- Block: 磁盘中的物理块
- blocks: 
- FAT: Fat表
- FCB:
- CatalogNode:
"""
#每个物理块大小
blockSize=512
#磁盘中物理块个数
blockNum=512

class Block:
    """
    磁盘中的一个个物理块
    """
    def __init__(self,blockIndex:int,data=""):
        #编号
        self.blockIndex=blockIndex
        #数据
        self.data=data
    
    def write(self,newData:str):
        self.data=newData[:blockSize]
        return newData[blockSize:]
    
    def read(self):
        return self.data

    def isFull(self):
        return len(self.data)==blockSize

    def append(self,newData:str)->str:
        """
        追加新内容，返回无法写入的部分
        """
        remainSpace=blockSize-len(self.data)
        if remainSpace>=newData:
            return ""
        else:
            self.data+=newData[:remainSpace]
            return newData[remainSpace:]
    
    def clear(self):
        self.data=""


class FAT:
    """
    显示链接FAT表
    """
    def __init__(self):
        self.fat=[]
        for i in range(blockNum):
            self.fat.append(-2)

    def findBlank(self):
        for i in range(blockNum):
            if self.fat[i]==-2:
                return i
        return -1
    
    def write(self,data,disk):
        """
        写数据，为其寻找到一个空闲的空间
        """
        
        start=-1
        cur=-1

        while data!="":
            newLoc=self.findBlank()
            if newLoc==-1:
                raise Exception(print('磁盘空间不足!'))
                return
            if cur!=-1:
                self.fat[cur]=newLoc
            else:
                start=newLoc
            cur=newLoc
            data=disk[cur].write(data)
            self.fat[cur]=-1

        return start
        
    
    def delete(self,start,disk):
        if start==-1:
            return

        while self.fat[start]!=-1:
            disk[start].clear()
            las=self.fat[start]
            self.fat[start]=-2
            start=las

        self.fat[start]=-2
        disk[start].clear()
    
    def update(self,start,data,disk):
        """
        清空以start开始的fat表以及磁盘空间
        然后再重新写入
        """
        self.delete(start, disk)
        return self.write(data, disk)

    def read(self,start,disk):
        """
        读取从start开始的文件内容
        """
        data=""
        while self.fat[start]!=-1:
            data+=disk[start].read()
            start=self.fat[start]
        data+=disk[start].read()
        return data
        

class FCB:
    def __init__(self,name,createTime,data,fat,disk):
        #文件名
        self.name=name
        #创建时间
        self.createTime=createTime
        #最后修改时间
        self.updateTime=self.createTime

        #根据data为其分配空间
        self.start=-1
    
    def update(self,newData,fat,disk):
        """
        更新文件内容
        """
        self.start=fat.update(self.start,newData,disk)
    
    def delete(self,fat,disk):
        """
        删除文件
        """
        fat.delete(self.start,disk)
    
    def read(self,fat,disk):
        if self.start==-1:
            return ""
        else:
            return fat.read(self.start,disk)


class CatalogNode:
    """
    多级目录结点
    """
    def __init__(self,name,isFile,fat,disk,createTime,parent=None,data=""):
        #路径名
        self.name=name
        #是否为文件类型
        self.isFile=isFile
        #父结点
        self.parent=parent
        #创建时间
        self.createTime=createTime
        #更新时间
        self.updateTime=self.createTime

        #文件夹类型
        if not self.isFile:
            self.children=[]
        else:
            self.data=FCB(name, createTime, data, fat, disk)
    
        