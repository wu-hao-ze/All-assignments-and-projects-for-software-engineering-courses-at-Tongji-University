# database.py文件创建与数据库的连接
from sqlalchemy import create_engine
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker

from .data.data import SQLALCHEMY_DATABASE_URL

# 数据库访问地址，这部分移到data.py中了
# SQLALCHEMY_DATABASE_URL = "sqlite:///./sql_app.db"
# SQLALCHEMY_DATABASE_URL = "mysql+pymysql://user:password@127.0.0.1:3306/db"
#使用pymysql作为驱动，db是数据库名称

# 启动引擎
engine = create_engine(
    SQLALCHEMY_DATABASE_URL,
    encoding="utf-8",
    echo=True
)

# 启动会话
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

# 返回一个类，后续作为数据库模型的基类(ORM模型)
Base = declarative_base()
