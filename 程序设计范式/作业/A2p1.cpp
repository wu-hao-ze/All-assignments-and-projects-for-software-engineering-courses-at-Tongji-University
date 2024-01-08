#include <iostream>
#include <vector>

using namespace std;

bool canWatchAllMovies(vector<vector<int>>& intervals) {
    // Sort the intervals
    int n = intervals.size();
    for (int i = 0; i < n - 1; ++i) {
        for (int j = 0; j < n - i - 1; ++j) {
            if (intervals[j][0] > intervals[j + 1][0]) {
                swap(intervals[j], intervals[j + 1]);
            }
        }
    }

    int endTime = 0;

    for (const vector<int>& interval : intervals) {
        int start = interval[0];
        int end = interval[1];

        if (start >= endTime) {
            endTime = end;
        }
        else {
            return false;
        }
    }

    return true;
}

int main() {
    int n;
    cout<< "Enter the number of movies(-1 to exit): "<<endl;
    while (cin >> n) {
        if (n == -1) { break; }
        cout<< "Enter the start and end times of each movie: "<<endl;
		vector<vector<int>> intervals;
        for (int i = 0; i < n; i++) {
			int a,b;
			cin>>a>>b;
			vector<int> temp;
			temp.push_back(a);
			temp.push_back(b);
			intervals.push_back(temp);
		}
		cout << (canWatchAllMovies(intervals)?("true"):("false")) << endl;
        cout << "Enter the number of movies(-1 to exit): " << endl;
	}

    return 0;
}
