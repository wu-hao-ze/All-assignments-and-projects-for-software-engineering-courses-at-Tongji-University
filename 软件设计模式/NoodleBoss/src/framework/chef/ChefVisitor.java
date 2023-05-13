package framework.chef;

import framework.container.Container;

public interface ChefVisitor {
    void visit(Container container);
}
