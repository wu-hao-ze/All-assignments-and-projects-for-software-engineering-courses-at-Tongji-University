# Pydantic可以基于Python的类型提示来进行数据验证。
# 使用schemas.py来保存Pydantic模型，以便和保存SQLAlchemy模型的models.py文件进行区分
# 数据模式schemas是FastAPI模块用于数据传递的对象，通过继承pydantic中的类建立
from datetime import datetime
from typing import Dict, List, Optional
from pydantic import BaseModel


class DishBase(BaseModel):
    name: str  # 菜品名
    description: Optional[str] = None
    flavor: Optional[str] = None
    price: float


class DishCreate(DishBase):
    # store_id: int # 创建订单的菜品不需要商家id，因为创建菜品的商家唯一确定
    pass


class DishChange(DishBase):
    id: int


class Dish(DishBase):
    id: int
    store_id: int
    icon: Optional[str] = None

    class Config:
        orm_mode = True


class DishOrder(BaseModel):
    id: int
    num: int


class ShopLogin(BaseModel):
    phone: str
    password: str


class ShopBase(BaseModel):
    name: str
    phone: str
    describe: Optional[str] = None
    address: Optional[str] = None


class ShopCreate(ShopBase):
    password: str


class Shop(ShopBase):
    id: int
    img: Optional[str] = None
    # is_active: bool
    # items: List[Dish] = []

    class Config:
        orm_mode = True


class ShopChange(BaseModel):
    name: str
    password: str
    describe: Optional[str] = None
    address: Optional[str] = None


class UserBase(BaseModel):
    pass


class UserCreate(UserBase):
    openid: str


class User(UserBase):
    id: int

    class Config:
        orm_mode = True


class OrderBase(BaseModel):
    store_id: int
    totalPrice: int = 0


class OrderCreate(OrderBase):
    countArray: List[DishOrder]


class Order(OrderBase):
    id: int
    shopImg: str
    shopName: str
    orderDesc: str
    orderComment: bool


class CommentBase(BaseModel):
    order_id: int
    user_text: str
    user_score: int


class CommentCreate(CommentBase):
    pass


class CommentReply(BaseModel):
    order_id: int
    store_text: str


class Comment(CommentBase):
    store_text: Optional[str] = None
    store_time: Optional[datetime] = None
    user_time: Optional[datetime] = None

    class Config:
        orm_mode = True


###################


class SimpleReply(BaseModel):
    msg: str

    class Config:
        orm_mode = True


class ShopDict(SimpleReply):
    shoplist: List[Dict]


class FlavorList(BaseModel):
    name: str
    foods: List[Dict]


class DishDict(SimpleReply):
    goods: List[FlavorList]


class DishItem(BaseModel):
    order_id: int
    store_id: int
    orderPrice: float
    orderShop: str
    orderImg: Optional[str] = None
    orderDesc: str
    orderComment: bool


class DishSimply(BaseModel):
    num: int
    name: str


class OrderDict(SimpleReply):
    data: List[DishItem]


class CommentDict(SimpleReply):
    data: Comment


class OrderShop(BaseModel):
    order_id: int
    user_id: int
    orderPrice: int
    orderDesc: List[DishSimply]
    submit_time: datetime
    finish_time: Optional[datetime] = None
    status: bool
    comment: bool
