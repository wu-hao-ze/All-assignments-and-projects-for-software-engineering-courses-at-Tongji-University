package framework.merch.single.beverage;

import framework.merch.single.SingleOrder;

public class OrangeJuice extends SingleOrder {
    public OrangeJuice(double price) {
        super(price);
    }

    @Override
    public String getName() {
        return "橙汁";
    }

    @Override
    public void handle() {
        System.out.println("新接了一杯橙汁！");
    }
}
