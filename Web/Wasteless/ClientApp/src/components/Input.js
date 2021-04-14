import React from 'react';

const input = ({id, changed, value, label, className, type}) => (
    <div className="form-group">
        <label>{label} </label>
        <input id={id} onChange={changed} value={value} className={className} type={type}/>
    </div>
);

export default input;