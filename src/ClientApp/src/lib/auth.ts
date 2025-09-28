export function saveAuthTokens(accessToken: string, refreshToken: string) {
  localStorage.setItem("accessToken", accessToken)
  localStorage.setItem("refreshToken", refreshToken)
}

export function clearAuth() {
  localStorage.removeItem("accessToken")
  localStorage.removeItem("refreshToken")
}

export function getAccessToken(): string | null {
  return localStorage.getItem("accessToken")
}

export function getRefreshToken(): string | null {
  return localStorage.getItem("refreshToken")
}
