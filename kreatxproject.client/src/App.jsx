import { useEffect, useState } from 'react';

function App() {
    const [projects, setProjects] = useState([]);

    useEffect(() => {
        // Kjo thirrje merr të dhënat nga API që rregulluam bashkë
        fetch('https://localhost:7246/api/projects')
            .then(response => response.json())
            .then(data => setProjects(data))
            .catch(error => console.error("Gabim:", error));
    }, []);

    return (
        <div style={{ padding: '20px', fontFamily: 'Arial' }}>
            <h1>Lista e Projekteve (Internship)</h1>
            <table border="1" cellPadding="10" style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                    <tr style={{ backgroundColor: '#f2f2f2' }}>
                        <th>ID</th>
                        <th>Emri i Projektit</th>
                        <th>Përshkrimi</th>
                    </tr>
                </thead>
                <tbody>
                    {projects.map(project => (
                        <tr key={project.id}>
                            <td>{project.id}</td>
                            <td>{project.name}</td>
                            <td>{project.description}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
            {projects.length === 0 && <p>Duke u ngarkuar ose tabela është bosh...</p>}
        </div>
    );
}

export default App;