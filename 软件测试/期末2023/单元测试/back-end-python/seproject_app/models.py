# models.py文件中是数据库的对应SQLAlchemy模型
# 模型models用于建立供SQLAlchemy模块增删改查使用的对象，继承SQLAlchemy中的数据模型基类，根据项目需求建立数据模型
# FastAPI是一个轻量级的框架，与数据库的通信是通过SQLAlchemy包来实现的
# 如果数据库数据已经存在，可以使用sqlacodegen直接生成每个表的model
# sqlacodegen --outfile=models.py mysql://user:password@127.0.0.1:3306/db
# 如果不指定outfile，则会直接在屏幕上输出

# 如果数据库数据不存在，则先在数据库中建表，然后通过以下代码对表进行完善
from sqlalchemy import Boolean, Column, DateTime, Float, ForeignKey, Integer, String
from sqlalchemy import CHAR, Column, DateTime, ForeignKey, Index, String
from sqlalchemy.dialects.mysql import INTEGER, LONGTEXT, SMALLINT, TINYINT
from sqlalchemy.orm import relationship
from .database import Base

class User(Base):
    __tablename__ = "user"

    id = Column(
        Integer, primary_key=True, index=True, autoincrement=True, nullable=False
    )
    open_id = Column(String(40), index=True, nullable=False)


class Shop(Base):
    __tablename__ = "store"

    id = Column(
        Integer, primary_key=True, index=True, autoincrement=True, nullable=False
    )
    name = Column(String(40))
    phone = Column(String(40), index=True, nullable=False)
    password = Column(String(40), nullable=False)
    describe = Column(String(40))
    address = Column(String(40))
    distance = Column(Integer, default=0)
    img = Column(String(10000))


class Order_Dish(Base):
    __tablename__ = "order-dish"

    order_id = Column(Integer, ForeignKey("order.id"), primary_key=True, nullable=False)
    dish_id = Column(Integer, ForeignKey("dish.id"), primary_key=True, nullable=False)
    num = Column(Integer, nullable=False)


class Dish(Base):
    __tablename__ = "dish"

    id = Column(
        Integer, primary_key=True, index=True, autoincrement=True, nullable=False
    )
    store_id = Column(Integer, ForeignKey("store.id"), nullable=False)
    name = Column(String(40))
    flavor = Column(String(40))
    price = Column(Float)
    description = Column(String(40))
    icon = Column(String(10000))


class Comment(Base):
    __tablename__ = "evaluate"

    order_id = Column(
        Integer, ForeignKey("order.id"), primary_key=True, index=True, nullable=False
    )
    user_text = Column(String(40))
    store_text = Column(String(40))
    user_time = Column(DateTime)
    store_time = Column(DateTime)
    user_score = Column(Integer)


class Order(Base):
    __tablename__ = "order"

    id = Column(
        Integer, primary_key=True, index=True, autoincrement=True, nullable=False
    )
    store_id = Column(Integer, ForeignKey("store.id"), nullable=False)
    user_id = Column(Integer, ForeignKey("user.id"), nullable=False)
    price = Column(Float, nullable=False)


class Order_Status(Base):
    __tablename__ = "order_status"

    order_id = Column(
        Integer, ForeignKey("order.id"), primary_key=True, index=True, nullable=False
    )
    submit_time = Column(DateTime)
    finish_time = Column(DateTime)
    status = Column(Boolean, default=False)
    comment = Column(Boolean, default=False)
