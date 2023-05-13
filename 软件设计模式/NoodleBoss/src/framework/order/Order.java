package framework.order;

import framework.merch.Merch;

import java.util.ArrayList;

/**
 * DP: Composite
 */
public class Order implements OrderInterface {
    private ArrayList<Merch> merches = new ArrayList<>();

    public void add(Merch merch) {
        merches.add(merch);
    }

    /**
     * DP: Iterator
     *
     * @return
     */
    @Override
    public double totalPrice() {
        double price = 0;
        for (Merch merch : merches) {
            price += merch.getPrice();
        }
        return price;
    }

    public void displayMerches() {
        System.out.print("订单的内容包括：");
        for (Merch merch : merches) {
            System.out.print(merch.getName() + " ");
        }
        System.out.println();
        System.out.println("订单的价格为：" + totalPrice());
    }

    public void handle() {
        for (Merch merch : merches) {
            System.out.println();
            merch.handle();
        }
        System.out.println();
    }

    public ArrayList<Merch> getMerches() {
        return merches;
    }

    @Override
    public boolean hasTray() {
        return false;
    }
}
