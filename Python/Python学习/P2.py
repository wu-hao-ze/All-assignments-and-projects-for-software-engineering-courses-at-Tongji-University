import math

import matplotlib.pyplot as plt
import numpy as np

# Data for plotting
t = np.arange(-3.14, 3.14, 0.01)

# Sine Plot
s1 = np.sin(math.pi * t)
s2 = np.cos(2 * math.pi * t)
s3 = np.cos(5 * math.pi * t)
plt.rcParams['axes.unicode_minus'] = False
fig, ax2 = plt.subplots()
fig, ax = plt.subplots()
ax2.plot(t, 2 * s1 + s3)
ax.plot(t, 5 * s1 * s2)
plt.show()
