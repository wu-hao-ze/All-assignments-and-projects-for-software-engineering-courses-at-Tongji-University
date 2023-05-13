def fun(d, r, S):
    Q = d // r
    L = d % r
    if Q == 0:
        S += [str(L)]

    else:
        fun(Q, r, S)
        S += [str(L)]


S = []
fun(10, 2, S)
print("".join(S))
