type = "mysql+pymysql"
username = "root"
password = "123456"
ipaddrsss = "127.0.0.1"
port = 3306
schema = "seproject"

SQLALCHEMY_DATABASE_URL = f"{type}://{username}:{password}@{ipaddrsss}:{port}/{schema}"
imgprefix = "http://127.0.0.1:8000/source/"
testopenid = "oxxHL5GS26iMW6iSMzfoLjXk1luw"
imgpath = "../wxfile/source/seproject/"

wxurl = "https://api.weixin.qq.com/sns/jscode2session"
wxappid = "wx27cc35bd452e6ec4"
wxsecret = "11dcbb7eb2d66961d8efe970de02c2fa"

# 在终端中通过uvicorn启动
# uvicorn seproject_app.main:app --reload