class identifier(object):
    def __init__(self, id):
        self.id = id

    def getyear(self):
        str1 = self.id[6:10]
        return str1

    def Disp(self):
        print(self.id)


n = identifier('210702199910214545')
print(n.getyear())
n.Disp()
