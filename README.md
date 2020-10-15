# FixedGameMath

[![Build Status](https://travis-ci.com/stellarLuminant/FixedGameMath.svg?branch=master)](https://travis-ci.com/stellarLuminant/FixedGameMath) 
[![codecov](https://codecov.io/gh/stellarLuminant/FixedGameMath/branch/master/graph/badge.svg)](https://codecov.io/gh/stellarLuminant/FixedGameMath)

###### Fork of [asik/FixedMath.Net](https://github.com/asik/FixedMath.Net).

A fixed-point numerics library that implements Fix64, a Q31.32 signed number alongside basic functions. This fork takes a more opinionated approach towards usage with a 2D game framework such as MonoGame.

### Features

- **Basic arithmetic:** Supports saturating addition, subtraction, multiplication, division. Also offers faster alternatives without overflow checks.
- **Basic trigonometry in degrees:** Currently supports sin(x), cos(x), tan(x), atan(x), atan2(y, x), and acos(x).
- **Exponential functions:** Currently supports Sqrt(x), Pow(x, y), Pow2(x), Log2(x), Ln(x).
- Explicit conversion between integral and floating point types.
- Full-precision parsing and printing with configurable digits and zero padding.

### Roadmap

- **Add basic geometric structs:** Reimplementations of common structs such as Rects or Vector2 would mean less work on the user to integrate this library.
- **Implement a determinism test suite:** Current test suite verifies functions by comparing them to verified implementations (such as System.Math floating point) with a small range of tolerance for error. A determinism test suite would instead store the outputs from a single target platform and compare them for bit-exactness on other platforms.

### License 

This project uses code derivative of:
- [asik/FixedMath.Net](https://github.com/asik/FixedMath.Net) (Apache V2.0)
- [dmoulding/log2fix](https://github.com/dmoulding/log2fix) (MIT)
- [PetteriAimonen/libfixmath](https://github.com/PetteriAimonen/libfixmath) (MIT)

Contributions are welcome!
