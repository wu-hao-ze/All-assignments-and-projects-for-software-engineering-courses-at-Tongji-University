package framework.time;


import java.util.ArrayList;

/**
 * DP: Singleton, Observer
 * 内部时间函数
 */
public class Timer {
    private Timer() {
        if (instance == null) {
            instance = this;
        } else {
            throw new IllegalStateException("Already initialized.");
        }
    }

    private static Timer instance;

    public static synchronized Timer getInstance() {
        if (instance == null) {
            instance = new Timer();
        }

        return instance;
    }

    private ArrayList<TimeObserver> observers = new ArrayList<>();

    /**
     * 更新内部时间，加快历史的进程
     */
    public void tick() {
        for (TimeObserver o : observers) {
            o.update();
        }
    }

    public void addObserver(TimeObserver o) {
        observers.add(o);
    }

    public void removeObserver(TimeObserver o) {
        observers.remove(o);
    }

}
