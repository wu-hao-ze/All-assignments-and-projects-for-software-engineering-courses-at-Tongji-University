class Person(object):
    def __init__(self, Name, Age):
        self.Name = Name
        self.Age = Age

    def Disp(self):
        print(self.Name)
        print(self.Age)


person = Person('whz', 18)
person.Disp()
