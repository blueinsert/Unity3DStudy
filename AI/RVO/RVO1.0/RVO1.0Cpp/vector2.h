/*! \file vector2.h Contains the Vector2 class; a two-dimensional vector, and some operations on a vector. */ 

#ifndef __VECTOR_H__
#define __VECTOR_H__

#include <iostream>
#include <cmath>

namespace RVO {
  /*! Defines a two-dimensional vector and its operations. */
  class Vector2 {

  private:
    /*! The x-component of the vector. */
    float _x;
    /*! The y-component of the vector. */
    float _y;
    
  public:
    /*! \returns The x-component of the vector. */
    inline float x() const { return _x; }
    /*! \returns The y-component of the vector. */
    inline float y() const { return _y; }
      
    /*! Constructs a null vector. */
    inline Vector2() { _x = 0; _y = 0; }
    /*! Copy constructor. 
        \param q The vector to be copied into the new vector. */
    inline Vector2(const Vector2& q) { _x = q.x(); _y = q.y(); }
    /*! Constructor. 
        \param x The x-component of the new vector. 
        \param y The y-component of the new vector. */
    inline Vector2(float x, float y) { _x = x; _y = y; }

    /*! Unary minus. 
        \returns The negation of the vector.  */
    inline Vector2 operator-() const { return Vector2(-_x, -_y); }
    /*! Unary plus. 
        \returns A reference to the vector.  */
    inline const Vector2& operator+() const { return *this; }

    /*! Dot product. 
        \param q The right hand side vector
        \returns The dot product of the lhs vector and the rhs vector.  */
    inline float operator*(const Vector2& q) const { return _x * q.x() + _y * q.y(); }
    /*! Scalar product. 
        \param a The right hand side scalar
        \returns The scalar product of the lhs vector and the rhs scalar.  */
    inline Vector2 operator*(float a) const { return Vector2(_x * a, _y * a); }
    /*! Scalar division. 
        \param a The right hand side scalar
        \returns The scalar division of the lhs vector and the rhs scalar.  */
    inline Vector2 operator/(float a) const { return Vector2(_x / a, _y / a); }
    /*! Vector addition. 
        \param q The right hand side vector
        \returns The sum of the lhs vector and the rhs vector.  */
    inline Vector2 operator+(const Vector2& q) const { return Vector2(_x + q.x(), _y + q.y()); }
    /*! Vector subtraction. 
        \param q The right hand side vector
        \returns The vector difference of the lhs vector and the rhs vector.  */
    inline Vector2 operator-(const Vector2& q) const { return Vector2(_x - q.x(), _y - q.y()); }
    
    /*! Vector equality. 
        \param q The right hand side vector
        \returns True if the lhs vector and the rhs vector are equal. False otherwise.  */
    inline bool operator==(const Vector2& q) const { return (_x == q.x() && _y == q.y()); }
    /*! Vector inequality. 
        \param q The right hand side vector
        \returns True if the lhs vector and the rhs vector are not equal. False otherwise.  */
    inline bool operator!=(const Vector2& q) const { return (_x != q.x() || _y != q.y()); }
    
    /*! The operator multiplies the vector by a scalar.
        \param a The scalar
        \returns A reference to the vector.  */
    inline const Vector2& operator*=(float a) { _x *= a; _y *= a; return *this; }
    /*! The operator divides the vector by a scalar.
        \param a The scalar
        \returns A reference to the vector.  */
    inline const Vector2& operator/=(float a) { _x /= a; _y /= a; return *this; }
    /*! The operator adds an rhs vector to the vector.
        \param q The right hand side vector 
        \returns A reference to the vector.  */
    inline const Vector2& operator+=(const Vector2& q) { _x += q.x(); _y += q.y(); return *this; }
    /*! The operator subtracts an rhs vector from the vector.
        \param q The right hand side vector 
        \returns A reference to the vector.  */
    inline const Vector2& operator-=(const Vector2& q) { _x -= q.x(); _y -= q.y(); return *this; }
  };
}

/*! Scalar multiplication. 
    \param a The left hand side scalar
    \param q The right hand side vector
    \returns The scalar multiplication of the lhs scalaer and the rhs vector.  */
inline RVO::Vector2 operator*(float a, const RVO::Vector2& q) { return RVO::Vector2(a * q.x(), a * q.y()); }

/*! Writes a vector to the standard output. 
    \param os The output stream
    \param q The vector
    \returns The standard output.  */
inline std::ostream& operator<<(std::ostream& os, const RVO::Vector2& q) {
  //os << "(" << q.x() << "," << q.y() << ")";
  os << q.x() << " " << q.y();
  return os;
}

/*! \param q A vector
    \returns The squared absolute value of the vector.  */
inline float absSq(const RVO::Vector2& q) { return q*q; }
/*! \param q A vector
    \returns The absolute value of the vector.  */
inline float abs(const RVO::Vector2& q) { return sqrt(absSq(q)); }
/*! \param q A vector
    \returns The normalized vector.  */
inline RVO::Vector2 norm(const RVO::Vector2& q) { return q / abs(q); }
/*! \param p A point
    \param q A point
    \returns The normal vector to the line segment pq. */
inline RVO::Vector2 normal(const RVO::Vector2& p, const RVO::Vector2& q) { return norm(RVO::Vector2(q.y() - p.y(), -(q.x() - p.x()))); }
/*! \param q A vector
    \returns The angle the vector makes with the positive x-axis. Is in the range [-PI, PI]. */
inline float atan(const RVO::Vector2& q) { return atan2(q.y(), q.x()); }
/*! \param p A vector
    \param q A vector
    \returns Returns the determinant of the 2x2 matrix formed by using p as the upper row and q as the lower row. */
inline float det(const RVO::Vector2& p, const RVO::Vector2& q) { return p.x()*q.y() - p.y()*q.x(); }

#endif