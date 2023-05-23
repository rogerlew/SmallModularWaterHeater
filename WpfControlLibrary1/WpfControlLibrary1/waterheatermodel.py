import numpy as np
import pylab

# Goh and Apt presented this 1st order ODE for modeling water temperature
# M*SHw*T' = UA*(Tamb-T) + Q
M = 50*128/16      # mass of water in tank (lb)
SHw = 0.999        # specific heat of water (BTU/lb/F)
UA = 4             # standby heat loss coefficient * area of storage tank (BTU/F/hr)
                   # 4 = 2.5" insulation; 2.5 = 3" insulation
Tamb = 62          # ambient temperature (F)
Qon = 4200*3.41214 # rate of heat input to tank from the heater (BTU/hr);
Qoff = 0           # zero when heater is off
T0 = 125.

def thermostat(Tcur, Tprev, setpoint=125, rng=3):
    low = setpoint - rng/2.
    high = setpoint + rng/2. 

    dT = Tcur-Tprev

    if dT > 0 and Tcur < high:
        return Qon
    else:
        if Tcur < low:
            return Qon
        else:
            return Qoff

duration = 24
N = 3600*duration
temp = np.zeros((N,))
temp[0] = T0 # initial temperature (F)
temp[1] = T0 # thermostat needs two samples
time = np.linspace(0, duration, N)
dt = time[1]-time[0]
Q  = np.zeros((N,))

for n,t in enumerate(time[1:-1]):
    # need to convert continuous ODE to discrete difference equation
    # this is possible by approximating derivatives with finite differences
    # see: http://web.eecs.utk.edu/~roberts/WebAppendices/Q-DiffEqs.pdf
    if t > 5 and t < 15:
        Q[n+2] = 0
    else:
        Q[n+2] = thermostat(temp[n+1],temp[n])

##    Q[n+2] = thermostat(temp[n+1],temp[n])
        
    temp[n+2] = (-UA*temp[n+1]+UA*Tamb+Q[n+2])*dt / (M*SHw) + temp[n+1]

kWh = (Q/3412.14)
kWh_cumsum = np.cumsum(kWh)/3600.

# plot results
pylab.figure()

pylab.subplot(311)
pylab.plot(time, temp)
pylab.title('Temperature')

pylab.subplot(312)
pylab.plot(time, kWh)
pylab.title('Instantaneous kWh')

pylab.subplot(313)
pylab.plot(time, kWh_cumsum)
pylab.title('Cumulative kWh')

pylab.savefig('setback.png')
pylab.close()

print kWh_cumsum[-1]
