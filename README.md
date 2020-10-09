# FixedGameMath
###### Fork of [asik/FixedMath.Net](https://github.com/asik/FixedMath.Net).

A fixed-point numerics library that implements Fix64, a Q31.32 signed number alongside basic functions. This fork takes a more opinionated approach towards usage with a 2D game framework such as MonoGame.

#### Tests on master: [![Build Status](https://travis-ci.com/stellarLuminant/FixedMath.NET.svg?branch=master)](https://travis-ci.com/stellarLuminant/FixedMath.NET)

### Features

- **Basic arithmetic:** Supports saturating addition, subtraction, multiplication, division. Also offers faster alternatives without overflow checks.
- **Basic trigonometry in degrees:** Currently supports sin(x), cos(x), tan(x), atan(x), atan2(y, x), and acos(x).
- **Exponential functions:** Currently supports Sqrt(x), Pow(x, y), Pow2(x), Log2(x), Ln(x).
- Explicit conversion between integral and floating point types.

### Roadmap

- **Add full-range precision parsing and printing:** Fix64 currently offers no native parsing solution, and casts to decimal in order to print.
- **Add basic geometric structs:** Reimplementations of common structs such as Rects or Vector2 would mean less work on the user to integrate this library.
- **Integrate test coverage analysis:** It would help to ensure all code paths are well-tested.
- **Implement a determinism test suite:** Current test suite verifies functions by comparing them to verified implementations (such as System.Math floating point) with a small range of tolerance for error. A determinism test suite would instead store the outputs from a single target platform and compare them for bit-exactness on other platforms.
