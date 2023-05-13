import matplotlib.pyplot as plt
import numpy as np

# Data for plotting
t = np.arange(0.0, 24, 0.01)

# Sine Plot
s = np.sin(t)
fig, ax = plt.subplots()
ax.plot(t, s)
ax.set(xlabel='radians', ylabel='Sin(x)',
       title='Sine Plot')
ax.grid()
plt.show()

# Cosine Plot
s = np.cos(t)
fig, ax = plt.subplots()
ax.plot(t, s)
ax.set(xlabel='radians', ylabel='(cosx)',
       title='Cosine Plot')
ax.grid()
plt.show()

# Tangent Plot
t = np.arange(0.0, 1, 0.01)
s = np.tan(t)
fig, ax = plt.subplots()
ax.plot(t, s)
ax.set(xlabel='radians', ylabel='tan(x)',
       title='Tangent Plot')
ax.grid()
plt.show()
