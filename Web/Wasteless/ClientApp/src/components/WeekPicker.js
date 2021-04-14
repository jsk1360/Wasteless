import React, {useEffect, useState} from "react";
import DayPickerInput from 'react-day-picker/DayPickerInput';
import 'react-day-picker/lib/style.css';
import './WeekPicker.css';
import moment from 'moment';
import MomentLocaleUtils, {formatDate, parseDate,} from 'react-day-picker/moment';

function getWeekDays(weekStart) {
    const days = [weekStart];
    for (let i = 1; i < 7; i += 1) {
        days.push(
            moment(weekStart)
                .add(i, 'days')
                .toDate()
        );
    }
    return days;
}

function getWeekRange(date) {
    return {
        from: moment(date)
            .startOf('isoWeek')
            .toDate(),
        to: moment(date)
            .endOf('isoWeek')
            .add(-2, 'day')
            .toDate(),
    };
}

export const WeekPicker = props => {
    const [hoverRange, setHoverRange] = useState(undefined);
    const [selectedDate, setSelectedDate] = useState();
    const [selectedDays, setSelectedDays] = useState([]);

    const onChange = props.onChange;

    useEffect(() => {
        setSelectedDays(getWeekDays(getWeekRange(moment()).from));
    }, [])

    useEffect(() => {
        if (selectedDays[0] !== selectedDate) {
            setSelectedDate(selectedDays[0]);
        }
    }, [selectedDays, selectedDate, onChange])

    useEffect(() => {
        onChange(selectedDays);
        console.log("onChange", selectedDays)
    }, [selectedDays, onChange])

    const handleDayChange = date => {
        const days = getWeekDays(getWeekRange(date).from)
        console.log(days);
        setSelectedDays(days);
    };

    const handleDayEnter = date => {
        setHoverRange(getWeekRange(date));
    };

    const handleDayLeave = () => {
        setHoverRange(undefined);
    };

    const handleWeekClick = (weekNumber, days, e) => {
        setSelectedDays(days);
    };

    const daysAreSelected = selectedDays.length > 0;
    const modifiers = {
        hoverRange,
        selectedRange: daysAreSelected && {
            from: selectedDays[0],
            to: selectedDays[4],
        },
        hoverRangeStart: hoverRange && hoverRange.from,
        hoverRangeEnd: hoverRange && hoverRange.to,
        selectedRangeStart: daysAreSelected && selectedDays[0],
        selectedRangeEnd: daysAreSelected && selectedDays[4],
        disabled: [{daysOfWeek: [0, 6]}]
    };

    return (
        <div className="SelectedWeek">
            <DayPickerInput
                inputProps={{className: "form-control week-picker"}}
                formatDate={formatDate}
                parseDate={parseDate}
                value={selectedDate}
                format="LL"
                dayPickerProps={{
                    locale: 'fi',
                    localeUtils: MomentLocaleUtils,
                    selectedDays: selectedDays,
                    modifiers: modifiers,
                    onDayMouseEnter: handleDayEnter,
                    onDayMouseLeave: handleDayLeave,
                    onWeekClick: handleWeekClick,
                    showWeekNumbers: true,
                    showOutsideDays: true,
                }}
                onDayChange={handleDayChange}/>
        </div>
    )
};