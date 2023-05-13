import matplotlib.pyplot as plt
import numpy as np

theta = np.linspace(0.0, 2 * np.pi, 1000)
a = 5
rho = a * (1 - np.sin(theta))
plt.subplot(polar=True)
plt.plot(theta, rho, c='r')
plt.show()
