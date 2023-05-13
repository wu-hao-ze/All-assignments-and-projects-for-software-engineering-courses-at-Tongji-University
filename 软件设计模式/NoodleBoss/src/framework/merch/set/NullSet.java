package framework.merch.set;

import framework.merch.single.SingleOrder;

import java.util.ArrayList;

public class NullSet extends Set {
    public NullSet() {
        super(new ArrayList<>());
    }

    @Override
    public String getName() {
        return "";
    }

    @Override
    public void handle() {

    }
}
