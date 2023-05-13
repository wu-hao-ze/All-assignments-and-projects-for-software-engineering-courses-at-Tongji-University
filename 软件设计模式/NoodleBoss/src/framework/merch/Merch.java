package framework.merch;

/**
 * DP:
 * 1.
 */
public abstract class Merch {
    private double price;

    public double getPrice() {
        return price;
    }

    public Merch(double price) {
        this.price = price;
    }

    public abstract String getName();

    public abstract void handle();

}
