package framework.container;

import framework.ingredient.Ingredient;
import framework.order.Order;
import framework.order.OrderInterface;

/**
 * DP:
 *  Decorator
 */
public class TrayDecorator implements OrderInterface {
    private Order order;

    public TrayDecorator(Order order) {
        this.order = order;
    }

    @Override
    public double totalPrice() {
        return order.totalPrice();
    }

    @Override
    public boolean hasTray() {
        return true;
    }

    @Override
    public void displayMerches() {
        System.out.println("本次订单已装盘");
        order.displayMerches();
    }
}
