import { useState, useEffect } from 'react';

const UserProfile = () => {
    const [fullName, setFullName] = useState('Name'); // 
    const [selectedFile, setSelectedFile] = useState(null);
    const [previewUrl, setPreviewUrl] = useState('');
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');
    const [isEditing, setIsEditing] = useState(false); // Kontrollon nese jemi ne modalitetin Edit apo View

    // Ngarkon foton dhe te dhenat e profilit kur hapet faqja
    useEffect(() => {
        let isMounted = true;

        const loadProfileData = async () => {
            try {
                const token = localStorage.getItem('token');

                // Marrim foton e profilit
                const picResponse = await fetch('https://localhost:7246/api/users/profile-picture', {
                    headers: { 'Authorization': `Bearer ${token}` }
                });

                if (picResponse.ok && isMounted) {
                    const picData = await picResponse.json();
                    if (picData && picData.path) {
                        setPreviewUrl(`https://localhost:7246${picData.path}`);
                    }
                }

                // Marrim te dhenat e profilit per emrin
                const profileResponse = await fetch('https://localhost:7246/api/users/profile', {
                    headers: { 'Authorization': `Bearer ${token}` }
                });

                if (profileResponse.ok && isMounted) {
                    const profileData = await profileResponse.json();
                    // Nese serveri kthen emrin (fullName ose username), e azhornojme
                    if (profileData && profileData.fullName) {
                        setFullName(profileData.fullName);
                    } else if (profileData && profileData.username) {
                        setFullName(profileData.username);
                    }
                }
            } catch (err) {
                console.error("Gabim gjate ngarkimit te profilit:", err);
            }
        };

        loadProfileData();

        return () => {
            isMounted = false;
        };
    }, []);

    // Rifreskon foton pas ruajtjes
    const refreshPictureAfterSave = async () => {
        try {
            const token = localStorage.getItem('token');
            const response = await fetch('https://localhost:7246/api/users/profile-picture', {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (response.ok) {
                const data = await response.json();
                if (data && data.path) {
                    setPreviewUrl(`https://localhost:7246${data.path}`);
                }
            }
        } catch (err) {
            console.error(err);
        }
    };

    // Ndryshimi i fotos ne menyre lokale
    const handleFileChange = (e) => {
        const file = e.target.files[0];
        if (file) {
            setSelectedFile(file);
            setPreviewUrl(URL.createObjectURL(file));
        }
    };

    // Dergimi i te dhenave te ndryshuara ne server
    const handleSubmit = async (e) => {
        e.preventDefault();
        setMessage('');
        setError('');

        if (!fullName.trim()) {
            setError('Full name is required.');
            return;
        }

        const formData = new FormData();
        formData.append('fullName', fullName);
        if (selectedFile) {
            formData.append('profilePicture', selectedFile);
        }

        try {
            const token = localStorage.getItem('token');
            const response = await fetch('https://localhost:7246/api/users/profile', {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` },
                body: formData
            });

            const textData = await response.text();
            const data = textData ? JSON.parse(textData) : {};

            if (response.ok) {
                setMessage('Profile updated successfully!');
                await refreshPictureAfterSave();
                setIsEditing(false); // Kthehemi automatikisht te pamja e thjeshte pas ruajtjes së suksesit
            } else {
                setError(data.message || 'Something went wrong.');
            }
        } catch (err) {
            console.error(err);
            setError('Error communicating with the server.');
        }
    };

    return (
        <div style={{ display: 'flex', justifyContent: 'center', marginTop: '30px' }}>
            <div style={{ backgroundColor: '#fff', padding: '30px', borderRadius: '8px', border: '1px solid #ddd', maxWidth: '420px', width: '100%', textAlign: 'center', boxShadow: '0 4px 6px rgba(0,0,0,0.05)' }}>

                <h2 style={{ marginBottom: '20px', color: '#333' }}>My Profile</h2>

                {message && <p style={{ color: 'green', backgroundColor: '#e6fffa', padding: '8px', borderRadius: '4px', fontSize: '14px' }}>{message}</p>}
                {error && <p style={{ color: 'red', backgroundColor: '#fff5f5', padding: '8px', borderRadius: '4px', fontSize: '14px' }}>{error}</p>}

                <form onSubmit={handleSubmit}>

                    {/* Rrethi i Fotos te Profilit */}
                    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', marginBottom: '20px' }}>
                        <div style={{ width: '120px', height: '120px', borderRadius: '50%', border: '3px solid #007bff', backgroundColor: '#f3f4f6', display: 'flex', justifyContent: 'center', alignItems: 'center', overflow: 'hidden', marginBottom: '12px' }}>
                            {previewUrl ? (
                                <img src={previewUrl} alt="Preview" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                            ) : (
                                <span style={{ color: '#aaa', fontSize: '13px' }}>No Image</span>
                            )}
                        </div>

                        {/* Butoni Change Photo shfaqet VETEM nese jemi ne modalitetin Edit */}
                        {isEditing && (
                            <>
                                <label htmlFor="file-upload" style={{ padding: '6px 12px', backgroundColor: '#f1f5f9', border: '1px solid #cbd5e1', borderRadius: '4px', cursor: 'pointer', fontSize: '13px', fontWeight: 'bold', color: '#475569' }}>
                                    Change Photo
                                </label>
                                <input id="file-upload" type="file" accept="image/*" onChange={handleFileChange} style={{ display: 'none' }} />
                            </>
                        )}
                    </div>

                    {/* Seksioni i Emrit */}
                    <div style={{ textAlign: 'center', marginBottom: '25px' }}>
                        {isEditing ? (
                            <div style={{ textAlign: 'left' }}>
                                <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold', color: '#555', fontSize: '14px' }}>Full Name:</label>
                                <input
                                    type="text"
                                    value={fullName}
                                    onChange={(e) => setFullName(e.target.value)}
                                    style={{ width: '100%', padding: '10px', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                                />
                            </div>
                        ) : (
                            <div>
                                <h3 style={{ margin: '0', color: '#222', fontSize: '22px' }}>{fullName}</h3>
                                <p style={{ margin: '5px 0 0 0', color: '#777', fontSize: '14px' }}>System User / Employee</p>
                            </div>
                        )}
                    </div>

                    {/* Ndryshimi i butonave ne baze te modalitetit (View apo Edit) */}
                    {isEditing ? (
                        <div style={{ display: 'flex', gap: '10px' }}>
                            <button type="button" onClick={() => setIsEditing(false)} style={{ width: '50%', padding: '11px', backgroundColor: '#6c757d', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold', cursor: 'pointer', fontSize: '14px' }}>
                                Cancel
                            </button>
                            <button type="submit" style={{ width: '50%', padding: '11px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold', cursor: 'pointer', fontSize: '14px' }}>
                                Save
                            </button>
                        </div>
                    ) : (
                        <button type="button" onClick={() => setIsEditing(true)} style={{ width: '100%', padding: '12px', backgroundColor: '#007bff', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold', cursor: 'pointer', fontSize: '15px' }}>
                            Edit Profile
                        </button>
                    )}
                </form>
            </div>
        </div>
    );
};

export default UserProfile;