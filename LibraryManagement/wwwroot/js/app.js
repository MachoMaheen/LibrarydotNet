// API Configuration
const API_BASE_URL = window.location.origin + '/api';

// Storage keys
const TOKEN_KEY = 'library_token';
const USER_KEY = 'library_user';

// Get stored auth data
function getAuth() {
    const token = localStorage.getItem(TOKEN_KEY);
    const user = localStorage.getItem(USER_KEY);
    return {
        token,
        user: user ? JSON.parse(user) : null
    };
}

// Set auth data
function setAuth(token, user) {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
}

// Clear auth data
function clearAuth() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
}

// Check if user is authenticated
function isAuthenticated() {
    return !!getAuth().token;
}

// Check if user has role
function hasRole(role) {
    const auth = getAuth();
    return auth.user && auth.user.role === role;
}

// Redirect if not authenticated
function requireAuth() {
    if (!isAuthenticated()) {
        window.location.href = '/login.html';
        return false;
    }
    return true;
}

// Redirect if not authorized
function requireRole(role) {
    if (!requireAuth()) return false;
    if (!hasRole(role)) {
        alert('You do not have permission to access this page.');
        window.location.href = '/index.html';
        return false;
    }
    return true;
}

// Logout
function logout() {
    clearAuth();
    window.location.href = '/login.html';
}

// API request helper
async function apiRequest(endpoint, method = 'GET', data = null) {
    const auth = getAuth();
    const headers = {
        'Content-Type': 'application/json'
    };

    if (auth.token) {
        headers['Authorization'] = `Bearer ${auth.token}`;
    }

    const config = {
        method,
        headers
    };

    if (data && (method === 'POST' || method === 'PUT')) {
        config.body = JSON.stringify(data);
    }

    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
        
        if (response.status === 401) {
            clearAuth();
            window.location.href = '/login.html';
            throw new Error('Unauthorized');
        }

        const result = await response.json();
        
        if (!response.ok) {
            throw new Error(result.message || 'Request failed');
        }

        return result;
    } catch (error) {
        console.error('API Error:', error);
        throw error;
    }
}

// Show alert message
function showAlert(message, type = 'info') {
    const alertDiv = document.getElementById('alert');
    if (!alertDiv) return;

    alertDiv.className = `alert alert-${type} show`;
    alertDiv.textContent = message;

    setTimeout(() => {
        alertDiv.className = 'alert';
    }, 5000);
}

// Show loading indicator
function showLoading(show = true) {
    const loadingDiv = document.getElementById('loading');
    if (!loadingDiv) return;

    if (show) {
        loadingDiv.classList.add('show');
    } else {
        loadingDiv.classList.remove('show');
    }
}

// Validate email
function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

// Validate form
function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form) return false;

    let isValid = true;
    const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');

    inputs.forEach(input => {
        const errorDiv = input.nextElementSibling;
        
        if (!input.value.trim()) {
            if (errorDiv && errorDiv.classList.contains('error')) {
                errorDiv.textContent = 'This field is required';
                errorDiv.classList.add('show');
            }
            isValid = false;
        } else if (input.type === 'email' && !validateEmail(input.value)) {
            if (errorDiv && errorDiv.classList.contains('error')) {
                errorDiv.textContent = 'Please enter a valid email';
                errorDiv.classList.add('show');
            }
            isValid = false;
        } else {
            if (errorDiv && errorDiv.classList.contains('error')) {
                errorDiv.classList.remove('show');
            }
        }
    });

    return isValid;
}

// Format date
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
        year: 'numeric', 
        month: 'short', 
        day: 'numeric' 
    });
}

// Format currency
function formatCurrency(amount) {
    return `$${parseFloat(amount).toFixed(2)}`;
}

// Update navbar with user info
function updateNavbar() {
    const auth = getAuth();
    const userInfo = document.getElementById('userInfo');
    
    if (userInfo && auth.user) {
        userInfo.innerHTML = `
            <span>Welcome, ${auth.user.name}</span>
            <a href="#" onclick="logout()">Logout</a>
        `;
    }
}

// Initialize page
document.addEventListener('DOMContentLoaded', () => {
    updateNavbar();
});
