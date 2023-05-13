class Person(object):
    def __init__(self, Name, Age):
        self.Name = Name
        self.Age = Age

    def Disp(self):
        print(self.Name)
        print(self.Age)


class Teacher(Person):
    def __init__(self, No, Ta):
        self.No = No
        self.Ta = Ta

    def NewDisp(self, Name, Age):
        super().__init__(Name, Age)
        super().Disp()
        print(self.No)
        print(self.Ta)


person = Person('whz', 18)
teacher = Teacher('12345', 20)
teacher.NewDisp(person.Name, person.Age)
