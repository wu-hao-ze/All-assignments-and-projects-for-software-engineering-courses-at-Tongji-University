# crud.py文件实现数据库的增删改查操作
import datetime
from typing import List

from sqlalchemy.orm import Session

from . import models, schemas


def get_shops(db: Session, skip: int = 0, limit: int = 100) -> List[models.Shop]:
    return db.query(models.Shop).offset(skip).limit(limit).all()


def get_shop_by_phone(db: Session, phone: str) -> models.Shop:
    return db.query(models.Shop).filter(models.Shop.phone == phone).first()


def get_shop_by_id(db: Session, id: int) -> models.Shop:
    return db.query(models.Shop).filter(models.Shop.id == id).first()


def get_dishs(db: Session, skip: int = 0, limit: int = 100) -> List[models.Dish]:
    return db.query(models.Dish).offset(skip).limit(limit).all()


def get_dishs_by_store_id(db: Session, store_id: str) -> List[models.Dish]:
    return db.query(models.Dish).filter(models.Dish.store_id == store_id).all()


def get_dish_by_id(db: Session, id: int) -> models.Dish:
    return db.query(models.Dish).filter(models.Dish.id == id).first()


# def get_evaluates(db: Session, skip: int = 0, limit: int = 100) -> List[models.Comment]:
#     return db.query(models.Comment).offset(skip).limit(limit).all()


# def get_orders(db: Session, skip: int = 0, limit: int = 100) -> List[models.Order]:
#     return db.query(models.Order).offset(skip).limit(limit).all()


def get_order_by_id(db: Session, id: int) -> models.Order:
    return db.query(models.Order).filter(models.Order.id == id).first()


# def get_orders_by_store_id(db: Session, store_id: int) -> List[models.Order]:
#     return db.query(models.Order).filter(models.Order.store_id == store_id).all()


# def get_order_dish(db: Session, skip: int = 0, limit: int = 100) -> List[models.Order_Dish]:
#     return db.query(models.Order_Dish).offset(skip).limit(limit).all()


# def get_order_status(db: Session, skip: int = 0, limit: int = 100) -> List[models.Order_Status]:
#     return db.query(models.Order_Status).offset(skip).limit(limit).all()


def get_user_by_openid(db: Session, openid: str) -> models.User:
    return db.query(models.User).filter(models.User.open_id == openid).first()


def get_comment_by_order_id(db: Session, order_id: int) -> models.Comment:
    return db.query(models.Comment).filter(models.Comment.order_id == order_id).first()


def get_comments_by_user_id(db: Session, user_id: int) -> List[models.Comment]:
    return (
        db.query(models.Comment)
        .join(models.Order)
        .filter(models.Order.user_id == user_id)
        .all()
    )


def get_comments_by_store_id(db: Session, store_id: int) -> List[models.Comment]:
    return (
        db.query(models.Comment)
        .join(models.Order)
        .filter(models.Order.store_id == store_id)
        .all()
    )


def get_orders_by_user_id(db: Session, user_id: int) -> list:
    results = (
        db.query(
            models.Order.id,
            models.Order.store_id,
            models.Order.price,
            models.Shop.name,
            models.Shop.img,
        )
        .filter(models.Order.user_id == user_id)
        .join(models.Shop)
        .all()
    )
    ans = []
    for i in results:
        sql = """
            SELECT `order-dish`.num, `dish`.name, `order_status`.comment
            FROM `order-dish`
            INNER JOIN `dish` ON `dish`.id = `order-dish`.dish_id
            INNER JOIN `order_status` ON `order_status`.order_id = `order-dish`.order_id
            WHERE `order-dish`.order_id = {}
        """.format(
            i[0]
        )
        res = db.execute(sql).fetchall()
        s = ""
        for j in res:
            s += f"{j[1]}×{j[0]}, "
        temp = {
            "order_id": i[0],
            "store_id": i[1],
            "orderPrice": i[2],
            "orderShop": i[3],
            "orderImg": i[4],
            "orderDesc": s,
            "orderComment": res[0][2],
        }
        ans.append(temp)
    return ans


def get_orders_by_store_id(db: Session, store_id: int) -> list:
    sql = """
        SELECT `order`.id, `order`.user_id, `order`.price, `order-dish`.num, `dish`.name
        FROM `order-dish`
        INNER JOIN `order` ON `order`.id = `order-dish`.order_id
        INNER JOIN `dish` ON `dish`.id = `order-dish`.dish_id
        WHERE `order`.store_id = {}
    """.format(
        store_id
    )
    res = db.execute(sql).fetchall()
    ans = []
    order = {}
    for i in res:
        if i[0] not in order:
            order[i[0]] = len(ans)
            t = {
                "order_id": i[0],
                "user_id": i[1],
                "orderPrice": i[2],
                "orderDesc": [
                    {
                        "num": i[3],
                        "name": i[4],
                    }
                ],
            }
            ans.append(t)
        else:
            t = {
                "num": i[3],
                "name": i[4],
            }
            ans[order[i[0]]]["orderDesc"].append(t)
    for i in ans:
        status = (
            db.query(models.Order_Status)
            .filter(models.Order_Status.order_id == i["order_id"])
            .first()
        )
        i["submit_time"] = status.submit_time
        i["finish_time"] = status.finish_time
        i["status"] = status.status
        i["comment"] = status.comment

    print(ans)
    return ans


# def get_comment_by_store_id(db: Session, store_id: int):
#     return db.query(models.Order).join(models.Order_Status).filter(models.Order.store_id == store_id)


def create_user(db: Session, openid: str) -> models.User:
    db_user = models.User(open_id=openid)
    db.add(db_user)
    db.commit()
    db.refresh(db_user)
    return db_user


def create_order(db: Session, user_id: int, order: schemas.OrderCreate) -> models.Order:
    db_order = models.Order(
        store_id=order.store_id, user_id=user_id, price=order.totalPrice
    )
    db.add(db_order)
    db.commit()
    db.refresh(db_order)
    return db_order.id


def create_order_status(db: Session, order_id: int):
    db_order_status = models.Order_Status(
        order_id=order_id, submit_time=datetime.datetime.now()
    )
    db.add(db_order_status)
    db.commit()


def create_shop(db: Session, shop: schemas.ShopCreate) -> models.Shop:
    # fake_hashed_password = shop.password + "notreallyhashed"
    db_shop = models.Shop(**shop.dict())
    db.add(db_shop)
    db.commit()
    db.refresh(db_shop)
    return db_shop


def create_comment(db: Session, comment: schemas.CommentCreate) -> models.Comment:
    db_comment = models.Comment(**comment.dict(), user_time=datetime.datetime.now())
    db.add(db_comment)
    db.commit()
    db.refresh(db_comment)
    change_order_comment(db, comment.order_id)
    return db_comment


def create_dish(db: Session, dish: schemas.DishCreate, shop_id: int) -> models.Dish:
    db_dish = models.Dish(**dish.dict(), store_id=shop_id)
    db.add(db_dish)
    db.commit()
    db.refresh(db_dish)
    return db_dish


def add_order_dish(db: Session, order_id: int, dishes: List[schemas.DishOrder]):
    for i in dishes:
        db_order_dish = models.Order_Dish(order_id=order_id, dish_id=i.id, num=i.num)
        db.add(db_order_dish)
        db.commit()


def change_shop_by_id(db: Session, id: int, shop: schemas.ShopChange) -> models.Shop:
    db.query(models.Shop).filter(models.Shop.id == id).update(shop.dict())
    db.commit()
    return db.query(models.Shop).filter(models.Shop.id == id).first()


def change_dish_by_id(
    db: Session, dish_id: int, dish: schemas.DishChange
) -> models.Dish:
    db.query(models.Dish).filter(models.Dish.id == dish_id).update(dish.dict())
    db.commit()
    return db.query(models.Dish).filter(models.Dish.id == dish_id).first()


def change_shop_img(db: Session, shop_id: int, imgaddress: str) -> models.Shop:
    db.query(models.Shop).filter(models.Shop.id == shop_id).update({models.Shop.img: imgaddress})
    db.commit()


def change_dish_img(db: Session, dish_id: int, imgaddress: str) -> models.Shop:
    db.query(models.Dish).filter(models.Dish.id == dish_id).update({models.Dish.icon: imgaddress})
    db.commit()


def change_order_status(db: Session, order_id: int):
    db.query(models.Order_Status).filter(
        models.Order_Status.order_id == order_id
    ).update(
        {
            models.Order_Status.status: (1 - models.Order_Status.status),
            models.Order_Status.finish_time: datetime.datetime.now(),
        }
    )
    db.commit()


def change_order_comment(db: Session, order_id: int):
    db.query(models.Order_Status).filter(
        models.Order_Status.order_id == order_id
    ).update({models.Order_Status.comment: (1 - models.Order_Status.comment)})
    db.commit()


def change_reply_comment(
    db: Session, order_id: int, reply: schemas.CommentReply
) -> models.Comment:
    d = reply.dict()
    d["store_time"] = datetime.datetime.now()
    db.query(models.Comment).filter(models.Comment.order_id == reply.order_id).update(d)
    db.commit()
    return (
        db.query(models.Comment)
        .filter(models.Comment.order_id == reply.order_id)
        .first()
    )
