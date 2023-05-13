package framework.container;

import framework.ingredient.Ingredient;

import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.NoSuchElementException;

public class ContainerIterator implements Iterator<Ingredient> {
    private ArrayDeque<Ingredient> queue = new ArrayDeque<>();

    ContainerIterator(ArrayList<Ingredient> ingredients) {
        for (Ingredient ingredient: ingredients) {
            queue.push(ingredient);
        }
    }

    public boolean hasNext() {
        return !queue.isEmpty();
    }

    public Ingredient next() throws NoSuchElementException {
        if (queue.isEmpty()) {
            throw new NoSuchElementException();
        }
        return queue.pollFirst();
    }
}
