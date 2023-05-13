package framework.merch.single.beverage;

import framework.merch.Merch;
import framework.merch.single.SingleOrder;


public class Coke extends Beverages{
    public Coke(double price) {
        super(price);
    }

    @Override
    public String getName() {
        return "可乐";
    }

    @Override
    public void handle() {
        System.out.println("新接了一杯可乐！");
    }
}
