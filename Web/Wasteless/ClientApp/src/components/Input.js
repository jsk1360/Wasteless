import React from 'react';

const input = ({id, changed, value, label, className, type, formGroup = true, readOnly = false}) => (
    <>
        {formGroup ? <div className="form-group">
            <label>{label} </label>
            <input id={id} onChange={changed} value={value ? value : ''} className={className} type={type} disabled={readOnly}/>
        </div> : <>
            <label>{label} </label>
            <input id={id} onChange={changed} value={value ? value : ''} className={className} type={type} disabled={readOnly}/>
        </>}
    </>);

export default input;