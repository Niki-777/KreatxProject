import { useState } from 'react';

const Login = ({ onLoginSuccess }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            // Thirrja e API-së së backend-it
            const response = await fetch('https://localhost:7246/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),
            });

            const data = await response.json();

            if (!response.ok) {
                // Nëse statusi nuk është 2xx (p.sh. 401 Unauthorized), shfaqim mesazhin nga backend-i
                throw new Error(data.message || 'Email ose fjalëkalim i gabuar.');
            }

            // Nëse login-i është i suksesshëm:
            // 1. Ruajmë Token-in dhe Rolin në LocalStorage të browser-it
            localStorage.setItem('token', data.token);
            localStorage.setItem('role', data.role);

            // 2. Njoftojmë App.jsx që të ndryshojë state-in dhe të shfaqë tabelën
            onLoginSuccess();

        } catch (err) {
            setError(err.message || 'Ndodhi një gabim gjatë lidhjes me serverin.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: '400px', margin: '100px auto', padding: '30px', border: '1px solid #e0e0e0', borderRadius: '8px', boxShadow: '0 4px 6px rgba(0,0,0,0.1)', fontFamily: 'Arial, sans-serif' }}>
            <h2 style={{ textAlign: 'center', marginBottom: '20px', color: '#333' }}>Kreatx Project</h2>

            {error && <p style={{ color: 'red', backgroundColor: '#fde8e8', padding: '10px', borderRadius: '4px', fontSize: '14px' }}>{error}</p>}

            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: '15px' }}>
                    <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold', color: '#555' }}>Email:</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        style={{ width: '100%', padding: '10px', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                        placeholder="admin@kreatx.com"
                        required
                    />
                </div>

                <div style={{ marginBottom: '20px' }}>
                    <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold', color: '#555' }}>Fjalëkalimi:</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        style={{ width: '100%', padding: '10px', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                        placeholder="••••••••"
                        required
                    />
                </div>

                <button
                    type="submit"
                    disabled={loading}
                    style={{ width: '100%', padding: '12px', backgroundColor: loading ? '#6c757d' : '#007bff', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold', cursor: loading ? 'not-allowed' : 'pointer', fontSize: '16px' }}
                >
                    {loading ? 'Duke u verifikuar...' : 'Hyr në Sistem'}
                </button>
            </form>
        </div>
    );
};

export default Login;