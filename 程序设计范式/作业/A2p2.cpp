#include <iostream>
#include <vector>
#include <algorithm>

using namespace std;

int minMovieTheaters(vector<vector<int>>& intervals) {
    int n = intervals.size();
    for (int i = 0; i < n - 1; ++i) {
        for (int j = 0; j < n - i - 1; ++j) {
            if (intervals[j][0] > intervals[j + 1][0]) {
                swap(intervals[j], intervals[j + 1]);
            }
        }
    }
    // Separate start times and end times into two separate vectors
    vector<int> startTimes;
    vector<int> endTimes;

    for (vector<int>& interval : intervals) {
        startTimes.push_back(interval[0]);
        endTimes.push_back(interval[1]);
    }

    // Sort both vectors
    sort(startTimes.begin(), startTimes.end());
    sort(endTimes.begin(), endTimes.end());

    int theaters = 0; // Number of theaters in use
    int minTheaters = 0; // Minimum theaters required
    int endIndex = 0; // Index for endTimes

    for (int i = 0; i < startTimes.size(); ++i) {
        theaters++; // Allocate a theater for the current movie

        // Check if any movie has ended by comparing with start times
        while (startTimes[i] >= endTimes[endIndex]) {
            theaters--; // Free up a theater as a movie has ended
            endIndex++;
        }

        minTheaters = max(minTheaters, theaters);
    }

    return minTheaters;
}

int main() {
    int n;
    cout << "Enter the number of movies(-1 to exit): " << endl;
    while (cin >> n) {
        if (n == -1) { break; }
        cout<< "Enter the start and end times of each movie: " << endl;
        vector<vector<int>> intervals;
        for (int i = 0; i < n; i++) {
            int a, b;
            cin >> a >> b;
            vector<int> temp;
            temp.push_back(a);
            temp.push_back(b);
            intervals.push_back(temp);
        }
        cout << minMovieTheaters(intervals) << endl;
        cout << "Enter the number of movies(-1 to exit): " << endl;
    }

    return 0;
}
