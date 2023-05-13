package framework.chef;

import framework.order.Order;

public interface ChefCommand {
    void showStock();
    void processOrder(Order order);
}
