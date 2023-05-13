package framework.merch.single;

public class NullSingleOrder extends SingleOrder {
    public NullSingleOrder() {
        super(0);
    }

    @Override
    public String getName() {
        return "";
    }

    @Override
    public void handle() {
    }
}
