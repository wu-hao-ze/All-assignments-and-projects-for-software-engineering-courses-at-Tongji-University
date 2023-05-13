package lazyLoading;
// Java program to illustrate
// virtual proxy in
// Lazy Loading Design Pattern
import java.util.List;
import java.util.ArrayList;

// 接口：员工信息表ContactList
// 两种实现类：一种是代理实现ProxyImpl，另一种是真正实现Impl
interface ContactList
{
    public List<Employee> getEmployeeList();
}

// 公司类
class Company {
    String companyName;
    String companyAddress;
    String companyContactNo;
    ContactList contactList;

    public Company(String companyName, String companyAddress,
                   String companyContactNo, ContactList contactList)
    {
        this.companyName = companyName;
        this.companyAddress = companyAddress;
        this.companyContactNo = companyContactNo;
        this.contactList = contactList;
    }

    public String getCompanyName()
    {
        return companyName;
    }
    public String getCompanyAddress()
    {
        return companyAddress;
    }
    public String getCompanyContactNo()
    {
        return companyContactNo;
    }
    public ContactList getContactList()
    {
        return contactList;
    }

}

// 真正实现
class ContactListImpl implements ContactList {
    // 调用类内单独定义的函数getEmpList()来实现接口getEmployeeList()
    public List<Employee> getEmployeeList()
    {
        return getEmpList();
    }

    // 类内单独定义的函数getEmpList，在这里真正将数据载入
    private static List<Employee> getEmpList()
    {

        List<Employee> empList = new ArrayList<Employee>(4);

        empList.add(new Employee("Wang", 2565.55, "SE"));
        empList.add(new Employee("Huang", 2257.4, "Manager"));
        empList.add(new Employee("Li", 3256.77, "G4"));
        empList.add(new Employee("Wu", 4875.54, "SSE"));

        return empList;
    }
}

// 代理实现
class ContactListProxyImpl implements ContactList {

    // 代理实现类在创建时并没有载入数据，即contactList为null
    private ContactList contactList;

    // 在数据被需要时调用getEmployeeList()函数来载入数据
    public List<Employee> getEmployeeList()
    {
        if (contactList == null) {
            System.out.println("Fetching list of employees");
            // 用真正实现ContactListImpl来载入数据
            contactList = new ContactListImpl();
        }
        return contactList.getEmployeeList();
    }
}

// 员工类
class Employee {
    private String employeeName;

    private double employeeSalary;
    private String employeeDesignation;

    public Employee(String employeeName,
                    double employeeSalary,
                    String employeeDesignation)
    {
        this.employeeName = employeeName;
        this.employeeSalary = employeeSalary;
        this.employeeDesignation = employeeDesignation;
    }
    public String getEmployeeName()
    {
        return employeeName;
    }
    public double getEmployeeSalary()
    {
        return employeeSalary;
    }
    public String getEmployeeDesignation()
    {
        return employeeDesignation;
    }
    public String toString()
    {
        return "Employee Name: " + employeeName + ", EmployeeDesignation : " + employeeDesignation + ", Employee Salary : " + employeeSalary;
    }
}

class LazyLoading {
    public static void main(String[] args)
    {
        // 使用代理实现ContactListProxyImpl创建时contactList为null，即数据未被需要时先不将数据载入
        ContactList contactList = new ContactListProxyImpl();
        Company company = new Company ("ABC", "China", "+86 123456789", contactList);

        System.out.println("Company Name: " + company.getCompanyName());
        System.out.println("Company Address: " + company.getCompanyAddress());
        System.out.println("Company Contact No.: " + company.getCompanyContactNo());

        System.out.println("Requesting for contact list");
        // 调用getEmployeeList()时，即数据被需要时才将数据载入
        List<Employee> empList = (company.getContactList()).getEmployeeList();

        for (Employee emp : empList) {
            System.out.println(emp);
        }
    }
}