import { useEffect, useState } from 'react';
import Login from './components/Login';

function App() {
    const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem('token'));
    const [projects, setProjects] = useState([]);
    const [users, setUsers] = useState([]);
    const [tasks, setTasks] = useState([]); // Ruajmë tasket e projektit të zgjedhur
    const [selectedProject, setSelectedProject] = useState(null); // Projekti që klikohet

    // Fushat për projektin
    const [projectName, setProjectName] = useState('');
    const [projectDesc, setProjectDesc] = useState('');

    // Fushat për përdoruesin
    const [userEmail, setUserEmail] = useState('');
    const [userPassword, setUserPassword] = useState('');
    const [userRole, setUserRole] = useState('Employee');

    // Fushat për taskun e ri
    const [taskTitle, setTaskTitle] = useState('');
    const [taskDesc, setTaskDesc] = useState('');
    const [assignedUser, setAssignedUser] = useState('');

    const [currentTab, setCurrentTab] = useState('projects');
    const isAdmin = localStorage.getItem('role') === 'Administrator';

    // 1. Merr projektet
    useEffect(() => {
        if (isLoggedIn) {
            const token = localStorage.getItem('token');
            fetch('https://localhost:7246/api/projects', {
                method: 'GET',
                headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' }
            })
                .then(response => response.ok ? response.json() : [])
                .then(data => setProjects(data))
                .catch(error => console.error("Gabim te projektet:", error));
        }
    }, [isLoggedIn]);

    // 2. Merr përdoruesit (Na duhen edhe për dropdown-in e caktimit të taskeve)
    useEffect(() => {
        if (isLoggedIn && isAdmin) {
            const token = localStorage.getItem('token');
            fetch('https://localhost:7246/api/users', {
                method: 'GET',
                headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' }
            })
                .then(response => response.ok ? response.json() : [])
                .then(data => setUsers(data))
                .catch(error => console.error("Gabim te përdoruesit:", error));
        }
    }, [isLoggedIn, isAdmin]);

    // 3. Merr tasket kur klikohet një projekt
    const handleSelectProject = (project) => {
        setSelectedProject(project);
        const token = localStorage.getItem('token');
        const projId = project.id || project.Id;

        fetch(`https://localhost:7246/api/tasks/project/${projId}`, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' }
        })
            .then(response => response.ok ? response.json() : [])
            .then(data => setTasks(data))
            .catch(error => console.error("Gabim gjatë marrjes së taskeve:", error));
    };

    const handleLogout = () => {
        localStorage.clear();
        setProjects([]);
        setUsers([]);
        setTasks([]);
        setSelectedProject(null);
        setIsLoggedIn(false);
    };

    const handleCreateProject = async (e) => {
        e.preventDefault();
        const token = localStorage.getItem('token');
        try {
            const response = await fetch('https://localhost:7246/api/projects', {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify({ name: projectName, description: projectDesc })
            });
            if (response.ok) {
                const newProject = await response.json();
                setProjects([...projects, newProject]);
                setProjectName(''); setProjectDesc('');
            }
        } catch (error) { console.error(error); }
    };

    const handleCreateUser = async (e) => {
        e.preventDefault();
        const token = localStorage.getItem('token');
        try {
            const response = await fetch('https://localhost:7246/api/users', {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify({ email: userEmail, password: userPassword, role: userRole })
            });
            const data = await response.json();
            if (response.ok) {
                alert(data.message);
                setUsers([...users, { email: userEmail, role: userRole }]);
                setUserEmail(''); setUserPassword('');
            } else { alert(data.message); }
        } catch (error) { console.error(error); }
    };

    // 4. Krijimi i një Tasku të Ri (Versioni i Sigurt)
    const handleCreateTask = async (e) => {
        e.preventDefault();
        const token = localStorage.getItem('token');

        const selectedUserObj = users.find(u => u.id === assignedUser || u.Id === assignedUser || u.email === assignedUser);
        const finalUserId = selectedUserObj ? (selectedUserObj.id || selectedUserObj.Id) : null;
        const currentProjId = selectedProject.id || selectedProject.Id;

        const newTaskPayload = {
            Title: taskTitle,
            Description: taskDesc,
            IsCompleted: false,
            ProjectId: currentProjId,
            AssignedToUserId: finalUserId
        };

        try {
            const response = await fetch('https://localhost:7246/api/tasks', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(newTaskPayload)
            });

            if (response.ok) {
                const createdTask = await response.json();
                setTasks([...tasks, createdTask]);
                setTaskTitle('');
                setTaskDesc('');
                setAssignedUser('');
            } else {
                const errorData = await response.text();
                alert("Serveri refuzoi krijimin: " + errorData);
            }
        } catch (error) {
            console.error("Gabim në Fetch:", error);
            alert("Ndodhi një gabim gjatë lidhjes me serverin.");
        }
    };

    // 5. Ndryshimi i Statusit të Taskut (Complete / Incomplete)
    const handleToggleTaskComplete = async (task) => {
        const token = localStorage.getItem('token');
        const taskId = task.id || task.Id;
        const currentStatus = task.isCompleted || task.IsCompleted || false;

        const updatedTask = {
            Id: taskId,
            Title: task.title || task.Title,
            Description: task.description || task.Description,
            ProjectId: task.projectId || task.ProjectId,
            AssignedToUserId: task.assignedToUserId || task.AssignedToUserId,
            IsCompleted: !currentStatus
        };

        try {
            const response = await fetch(`https://localhost:7246/api/tasks/${taskId}`, {
                method: 'PUT',
                headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify(updatedTask)
            });

            if (response.ok) {
                setTasks(tasks.map(t => (t.id === taskId || t.Id === taskId) ? { ...t, isCompleted: !currentStatus, IsCompleted: !currentStatus } : t));
            }
        } catch (error) { console.error(error); }
    };

    if (!isLoggedIn) {
        return <Login onLoginSuccess={() => setIsLoggedIn(true)} />;
    }

    return (
        <div style={{ padding: '20px', fontFamily: 'Arial, sans-serif', maxWidth: '1000px', margin: '0 auto' }}>
            {/* Header */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                <h2>Paneli i Menaxhimit (Kreatx)</h2>
                <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
                    <span style={{ fontSize: '14px', color: '#666', backgroundColor: '#e9ecef', padding: '5px 10px', borderRadius: '4px' }}>
                        Roli: <strong>{localStorage.getItem('role')}</strong>
                    </span>
                    <button onClick={handleLogout} style={{ padding: '10px 15px', backgroundColor: '#dc3545', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold' }}>Logout</button>
                </div>
            </div>

            {/* Navigimi për Admin */}
            {isAdmin && (
                <div style={{ marginBottom: '20px', display: 'flex', gap: '10px' }}>
                    <button onClick={() => { setCurrentTab('projects'); setSelectedProject(null); }} style={{ padding: '10px 20px', backgroundColor: currentTab === 'projects' ? '#007bff' : '#6c757d', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold' }}>Menaxho Projektet</button>
                    <button onClick={() => setCurrentTab('users')} style={{ padding: '10px 20px', backgroundColor: currentTab === 'users' ? '#007bff' : '#6c757d', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold' }}>Menaxho Përdoruesit</button>
                </div>
            )}

            {/* TABI PROJEKTET */}
            {currentTab === 'projects' && (
                <div>
                    {isAdmin && !selectedProject && (
                        <div style={{ backgroundColor: '#f8f9fa', padding: '20px', borderRadius: '8px', marginBottom: '30px', border: '1px solid #e2e8f0' }}>
                            <h3 style={{ marginTop: 0, marginBottom: '15px' }}>Krijo Projekt të Ri</h3>
                            <form onSubmit={handleCreateProject} style={{ display: 'flex', gap: '15px', alignItems: 'flex-end', flexWrap: 'wrap' }}>
                                <div style={{ flex: '1' }}><label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>Emri:</label><input type="text" value={projectName} onChange={(e) => setProjectName(e.target.value)} required style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid #ccc' }} /></div>
                                <div style={{ flex: '2' }}><label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>Përshkrimi:</label><input type="text" value={projectDesc} onChange={(e) => setProjectDesc(e.target.value)} style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid #ccc' }} /></div>
                                <button type="submit" style={{ padding: '9px 20px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold', cursor: 'pointer' }}>Shto</button>
                            </form>
                        </div>
                    )}

                    {!selectedProject ? (
                        /* TABELA 1: LISTA E PROJEKTEVE KRYESORE */
                        <div>
                            <h3 style={{ color: '#333', marginBottom: '5px' }}>Projektet Aktuale</h3>
                            <p style={{ color: '#666', fontSize: '14px', marginBottom: '15px' }}>* Kliko mbi rreshtin e një projekti për të parë dhe menaxhuar tasket e tij.</p>
                            <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
                                <thead>
                                    <tr style={{ backgroundColor: '#f2f2f2' }}>
                                        <th style={{ padding: '12px', border: '1px solid #ddd' }}>ID</th>
                                        <th style={{ padding: '12px', border: '1px solid #ddd' }}>Emri i Projektit</th>
                                        <th style={{ padding: '12px', border: '1px solid #ddd' }}>Përshkrimi</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {projects.map(p => (
                                        <tr key={p.id || p.Id} onClick={() => handleSelectProject(p)} style={{ borderBottom: '1px solid #ddd', cursor: 'pointer', transition: 'background 0.2s' }} onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#f1f5f9'} onMouseOut={(e) => e.currentTarget.style.backgroundColor = 'transparent'}>
                                            <td style={{ padding: '12px', border: '1px solid #ddd' }}>{p.id || p.Id}</td>
                                            <td style={{ padding: '12px', border: '1px solid #ddd', fontWeight: 'bold', color: '#007bff' }}>{p.name || p.Name || 'Pa emër'} ↗</td>
                                            <td style={{ padding: '12px', border: '1px solid #ddd' }}>{p.description || p.Description || '-'}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    ) : (
                        /* TABELA 2: LISTA E TASKEVE TË PROJEKTIT TË ZGJEDHUR */
                        <div>
                            <button onClick={() => setSelectedProject(null)} style={{ marginBottom: '20px', padding: '8px 15px', backgroundColor: '#6c757d', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>← Kthehu te Projektet</button>
                            <h3 style={{ marginBottom: '5px' }}>Tasket për projektin: <span style={{ color: '#007bff' }}>{selectedProject.name || selectedProject.Name}</span></h3>

                            {/* Forma e shtimit të Taskut (Vetëm për Admin) */}
                            {isAdmin && (
                                <div style={{ backgroundColor: '#f8f9fa', padding: '15px', borderRadius: '8px', marginBottom: '20px', border: '1px solid #ddd' }}>
                                    <h4>Shto Task të Ri</h4>
                                    <form onSubmit={handleCreateTask} style={{ display: 'flex', gap: '15px', alignItems: 'flex-end', flexWrap: 'wrap' }}>
                                        <div style={{ flex: '1' }}><label style={{ display: 'block', fontSize: '13px' }}>Titulli:</label><input type="text" value={taskTitle} onChange={(e) => setTaskTitle(e.target.value)} required style={{ width: '100%', padding: '6px' }} /></div>
                                        <div style={{ flex: '1' }}><label style={{ display: 'block', fontSize: '13px' }}>Përshkrimi:</label><input type="text" value={taskDesc} onChange={(e) => setTaskDesc(e.target.value)} style={{ width: '100%', padding: '6px' }} /></div>
                                        <div style={{ width: '180px' }}>
                                            <label style={{ display: 'block', fontSize: '13px' }}>Cakto Përdoruesin:</label>
                                            <select value={assignedUser} onChange={(e) => setAssignedUser(e.target.value)} style={{ width: '100%', padding: '6px' }}>
                                                <option value="">Pa caktuar</option>
                                                {users.filter(u => u.role === 'Employee' || u.Role === 'Employee').map((u, i) => (
                                                    <option key={i} value={u.email || u.Email}>{u.email || u.Email}</option>
                                                ))}
                                            </select>
                                        </div>
                                        <button type="submit" style={{ padding: '7px 15px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold' }}>Shto Task</button>
                                    </form>
                                </div>
                            )}

                            {/* Tabela e Taskeve */}
                            <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
                                <thead>
                                    <tr style={{ backgroundColor: '#f2f2f2' }}>
                                        <th style={{ padding: '10px', border: '1px solid #ddd' }}>Statusi</th>
                                        <th style={{ padding: '10px', border: '1px solid #ddd' }}>Titulli</th>
                                        <th style={{ padding: '10px', border: '1px solid #ddd' }}>Përshkrimi</th>
                                        <th style={{ padding: '10px', border: '1px solid #ddd' }}>Veprimi</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {tasks.length > 0 ? (
                                        tasks.map(t => {
                                            const isComp = t.isCompleted || t.IsCompleted || false;
                                            return (
                                                <tr key={t.id || t.Id} style={{ borderBottom: '1px solid #ddd', backgroundColor: isComp ? '#f0fff4' : 'transparent' }}>
                                                    <td style={{ padding: '10px', border: '1px solid #ddd' }}>
                                                        <span style={{ fontWeight: 'bold', color: isComp ? '#28a745' : '#dc3545' }}>
                                                            {isComp ? "✓ I përfunduar" : "⏳ Në proces"}
                                                        </span>
                                                    </td>
                                                    <td style={{ padding: '10px', border: '1px solid #ddd', textDecoration: isComp ? 'line-through' : 'none' }}>{t.title || t.Title}</td>
                                                    <td style={{ padding: '10px', border: '1px solid #ddd', color: '#555' }}>{t.description || t.Description}</td>

                                                    <td style={{ padding: '10px', border: '1px solid #ddd' }}>
                                                        {!isAdmin ? (
                                                            <button onClick={() => handleToggleTaskComplete(t)} style={{ padding: '5px 10px', backgroundColor: isComp ? '#6c757d' : '#007bff', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '12px' }}>
                                                                {isComp ? "Shëno si Në proces" : "Shëno si i Kryer"}
                                                            </button>
                                                        ) : (
                                                            <span style={{ color: '#888', fontSize: '12px', fontStyle: 'italic' }}>Vetëm për Punonjësit</span>
                                                        )}
                                                    </td>
                                                </tr>
                                            );
                                        })
                                    ) : (
                                        <tr><td colSpan="4" style={{ padding: '15px', textAlign: 'center', color: '#888' }}>Nuk ka asnjë task për këtë projekt.</td></tr>
                                    )}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            )}

            {/* TABI PËRDORUESIT (VETËM ADMIN) */}
            {currentTab === 'users' && isAdmin && (
                <div>
                    <div style={{ backgroundColor: '#f8f9fa', padding: '20px', borderRadius: '8px', marginBottom: '30px', border: '1px solid #e2e8f0' }}>
                        <h3 style={{ marginTop: 0, marginBottom: '15px' }}>Krijo Përdorues të Ri (Punonjës)</h3>
                        <form onSubmit={handleCreateUser} style={{ display: 'flex', gap: '15px', alignItems: 'flex-end', flexWrap: 'wrap' }}>
                            <div style={{ flex: '1' }}><label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>Email:</label><input type="email" value={userEmail} onChange={(e) => setUserEmail(e.target.value)} required style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid #ccc' }} placeholder="punonjesi@kreatx.com" /></div>
                            <div style={{ flex: '1' }}><label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>Password:</label><input type="password" value={userPassword} onChange={(e) => setUserPassword(e.target.value)} required style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid #ccc' }} placeholder="Min. 6 karaktere" /></div>
                            <div style={{ width: '150px' }}>
                                <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>Roli:</label>
                                <select value={userRole} onChange={(e) => setUserRole(e.target.value)} style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid #ccc' }}>
                                    <option value="Employee">Employee</option>
                                    <option value="Administrator">Administrator</option>
                                </select>
                            </div>
                            <button type="submit" style={{ padding: '9px 20px', backgroundColor: '#007bff', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold', cursor: 'pointer' }}>Krijo Përdorues</button>
                        </form>
                    </div>

                    <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
                        <thead>
                            <tr style={{ backgroundColor: '#f2f2f2' }}>
                                <th style={{ padding: '12px', border: '1px solid #ddd' }}>Email</th>
                                <th style={{ padding: '12px', border: '1px solid #ddd' }}>Roli</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map((u, index) => (
                                <tr key={index} style={{ borderBottom: '1px solid #ddd' }}>
                                    <td style={{ padding: '12px', border: '1px solid #ddd' }}>{u.email || u.Email}</td>
                                    <td style={{ padding: '12px', border: '1px solid #ddd' }}>
                                        <span style={{ padding: '3px 8px', borderRadius: '12px', fontSize: '12px', fontWeight: 'bold', backgroundColor: (u.role === 'Administrator' || u.Role === 'Administrator') ? '#f8d7da' : '#d1ecf1', color: (u.role === 'Administrator' || u.Role === 'Administrator') ? '#721c24' : '#0c5460' }}>
                                            {u.role || u.Role}
                                        </span>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}

export default App;