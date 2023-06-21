import React, {useEffect, useState} from "react";
import Select from 'react-select'

export const MenuPicker = (props) => {
    const {menus, selected, onChange} = props;
    const [options, setOptions] = useState([]);
    const [selectedValue, setSelectedValue] = useState(undefined);

    useEffect(() => {
        const customOptions = [
            {
                locationId: menus.length > 0 ? menus[0].locationId : -1,
                name: "Muu",
            },
            ...menus,
        ];
        setOptions(customOptions);
        console.log('selected', selected);
        if (selected) {
            const foundValue = customOptions.find(o => o.name === selected.name);
            setSelectedValue(foundValue);
        }
    }, [menus, selected])

    function fuzzyMatch(pattern, str) {
        pattern = '.*' + pattern.split('').join('.*') + '.*';
        const re = new RegExp(pattern, 'i');
        return re.test(str);
    }

    const filterOptions = (candidate, input) => {
        if (input) {
            if (candidate.label === options[0].name) return true;

            return fuzzyMatch(input, candidate.label);
        }
        return true;
    };

    useEffect(() => {
        onChange(selectedValue);
    }, [selectedValue, onChange]);

    const handleSelect = (selection) => {
        console.log(selection);
        setSelectedValue(selection);
    }

    return (
        <Select isDisabled={true} isSearchable={false} value={selectedValue} options={options} filterOption={filterOptions}
                getOptionLabel={x => x.name} getOptionValue={x => x.name} onChange={handleSelect}/>
    )
}